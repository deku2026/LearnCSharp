using Microsoft.Data.Sqlite;

namespace LearnMaui.Maui_Stage12_LocalStorageOffline;

public sealed record Note(
    Guid Id,
    string Title,
    string Body,
    DateTimeOffset UpdatedAt,
    long Version);

public interface IOfflineNoteRepository
{
    string DatabasePath { get; }

    Task InitializeAsync(CancellationToken cancellationToken);

    Task SaveAsync(Note note, CancellationToken cancellationToken);

    Task<IReadOnlyList<Note>> ListAsync(CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<int> GetPendingOperationCountAsync(CancellationToken cancellationToken);

    Task<int> MarkAllSyncedAsync(CancellationToken cancellationToken);
}

public sealed class OfflineNoteRepository(string databasePath) : IOfflineNoteRepository
{
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public string DatabasePath { get; } = databasePath;

    private string ConnectionString =>
        new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
        }.ToString();

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        await _initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
            await using SqliteConnection connection = new(ConnectionString);
            await connection.OpenAsync(cancellationToken);
            await ExecuteAsync(connection, "PRAGMA journal_mode=WAL;", cancellationToken);
            await ExecuteAsync(
                connection,
                """
                CREATE TABLE IF NOT EXISTS notes (
                    id TEXT PRIMARY KEY NOT NULL,
                    title TEXT NOT NULL,
                    body TEXT NOT NULL,
                    updated_utc TEXT NOT NULL,
                    version INTEGER NOT NULL,
                    is_deleted INTEGER NOT NULL DEFAULT 0
                );
                CREATE INDEX IF NOT EXISTS ix_notes_updated
                    ON notes(is_deleted, updated_utc DESC);
                CREATE TABLE IF NOT EXISTS outbox (
                    operation_id TEXT PRIMARY KEY NOT NULL,
                    entity_id TEXT NOT NULL,
                    operation TEXT NOT NULL,
                    created_utc TEXT NOT NULL,
                    attempt_count INTEGER NOT NULL DEFAULT 0
                );
                PRAGMA user_version=1;
                """,
                cancellationToken);
            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async Task SaveAsync(Note note, CancellationToken cancellationToken)
    {
        await InitializeAsync(cancellationToken);
        await using SqliteConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using SqliteTransaction transaction =
            (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        await using (SqliteCommand noteCommand = connection.CreateCommand())
        {
            noteCommand.Transaction = transaction;
            noteCommand.CommandText =
                """
                INSERT INTO notes(id, title, body, updated_utc, version, is_deleted)
                VALUES($id, $title, $body, $updated, $version, 0)
                ON CONFLICT(id) DO UPDATE SET
                    title = excluded.title,
                    body = excluded.body,
                    updated_utc = excluded.updated_utc,
                    version = excluded.version,
                    is_deleted = 0;
                """;
            noteCommand.Parameters.AddWithValue("$id", note.Id.ToString("D"));
            noteCommand.Parameters.AddWithValue("$title", note.Title);
            noteCommand.Parameters.AddWithValue("$body", note.Body);
            noteCommand.Parameters.AddWithValue("$updated", note.UpdatedAt.ToString("O"));
            noteCommand.Parameters.AddWithValue("$version", note.Version);
            await noteCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        await EnqueueAsync(connection, transaction, note.Id, "upsert", cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Note>> ListAsync(CancellationToken cancellationToken)
    {
        await InitializeAsync(cancellationToken);
        await using SqliteConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, title, body, updated_utc, version
            FROM notes
            WHERE is_deleted = 0
            ORDER BY updated_utc DESC;
            """;
        await using SqliteDataReader reader =
            await command.ExecuteReaderAsync(cancellationToken);
        List<Note> notes = [];
        while (await reader.ReadAsync(cancellationToken))
        {
            notes.Add(new Note(
                Guid.Parse(reader.GetString(0)),
                reader.GetString(1),
                reader.GetString(2),
                DateTimeOffset.Parse(reader.GetString(3)),
                reader.GetInt64(4)));
        }

        return notes;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await InitializeAsync(cancellationToken);
        await using SqliteConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using SqliteTransaction transaction =
            (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);
        await using (SqliteCommand command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText =
                """
                UPDATE notes
                SET is_deleted = 1,
                    updated_utc = $updated,
                    version = version + 1
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$id", id.ToString("D"));
            command.Parameters.AddWithValue("$updated", DateTimeOffset.UtcNow.ToString("O"));
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await EnqueueAsync(connection, transaction, id, "delete", cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<int> GetPendingOperationCountAsync(
        CancellationToken cancellationToken)
    {
        await InitializeAsync(cancellationToken);
        await using SqliteConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM outbox;";
        object? count = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(count);
    }

    public async Task<int> MarkAllSyncedAsync(CancellationToken cancellationToken)
    {
        await InitializeAsync(cancellationToken);
        await using SqliteConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM outbox;";
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task EnqueueAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        Guid entityId,
        string operation,
        CancellationToken cancellationToken)
    {
        await using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            INSERT INTO outbox(operation_id, entity_id, operation, created_utc)
            VALUES($operationId, $entityId, $operation, $created);
            """;
        command.Parameters.AddWithValue("$operationId", Guid.NewGuid().ToString("D"));
        command.Parameters.AddWithValue("$entityId", entityId.ToString("D"));
        command.Parameters.AddWithValue("$operation", operation);
        command.Parameters.AddWithValue("$created", DateTimeOffset.UtcNow.ToString("O"));
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task ExecuteAsync(
        SqliteConnection connection,
        string sql,
        CancellationToken cancellationToken)
    {
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
