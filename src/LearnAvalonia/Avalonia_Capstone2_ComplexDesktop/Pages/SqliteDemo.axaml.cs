// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §6
// Title:  SQLite 本地索引
// Stage:  Capstone2
// Accept: 元数据 + 启动迁移 + 备份
// Notes:  路径在用户数据目录；本地为源 of truth 离线优先 或 服务器优先 选一种写 README。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class SqliteDemo : UserControl
{
    public SqliteDemo()
    {
        InitializeComponent();
    }

    private void MigrateButton_Click(object? sender, RoutedEventArgs e)
    {
        // 演示启动迁移 schema_version 表（附录 B）+ 建元数据表，:memory: 不写盘。
        using SqliteConnection conn = new("Data Source=:memory:");
        conn.Open();
        using (SqliteCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE schema_version (version INTEGER NOT NULL); INSERT INTO schema_version VALUES (1); CREATE TABLE recent_files (id INTEGER PRIMARY KEY, path TEXT, opened_at TEXT);";
            cmd.ExecuteNonQuery();
        }
        using SqliteCommand q = conn.CreateCommand();
        q.CommandText = "SELECT version FROM schema_version";
        object ver = q.ExecuteScalar() ?? 0;
        if (SqliteLog is not null)
        {
            SqliteLog.Text = $"启动迁移完成（:memory: 演示）：\n  schema_version = {ver}\n  表：schema_version, recent_files\n\n路径（生产）：\n  桌面 LocalApplicationData/MyApp/app.db\n  移动 个人库目录\n  Browser 特殊（WASM 或不用文件）";
        }
    }

    private void BackupButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SqliteLog is not null)
        {
            SqliteLog.Text = "备份命令：导出 DB 或 JSON（用户可导出加密可选）\n导入校验版本\n使用 IFilePickerService 保存\n\n与远程 API：本地为源 of truth 离线优先，或服务器优先 —— 选一种写进 README";
        }
    }
}
