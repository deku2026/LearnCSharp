using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LearnMaui.Maui_Stage06_BindingMVVM;

public enum NotesUiState
{
    Idle,
    Loading,
    Content,
    Empty,
    Error,
}

public sealed record NoteItem(Guid Id, string Title, DateTimeOffset UpdatedAt);

public interface INotesService
{
    Task<IReadOnlyList<NoteItem>> SearchAsync(string query, CancellationToken cancellationToken);
}

public sealed class InMemoryNotesService : INotesService
{
    private static readonly NoteItem[] Notes =
    [
        new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Compiled bindings", DateTimeOffset.Now),
        new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Async commands", DateTimeOffset.Now),
        new(Guid.Parse("00000000-0000-0000-0000-000000000003"), "Explicit UI states", DateTimeOffset.Now),
    ];

    public async Task<IReadOnlyList<NoteItem>> SearchAsync(
        string query,
        CancellationToken cancellationToken)
    {
        await Task.Delay(120, cancellationToken);
        return Notes
            .Where(note => note.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}

public sealed class NoteSavedMessage(Guid id) : ValueChangedMessage<Guid>(id);

public partial class NotesViewModel(INotesService notesService) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private NotesUiState _state = NotesUiState.Idle;

    [ObservableProperty]
    private NoteItem? _selected;

    public bool IsNotBusy => !IsBusy;

    public ObservableCollection<NoteItem> Items { get; } = [];

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            State = NotesUiState.Loading;
            IReadOnlyList<NoteItem> result =
                await notesService.SearchAsync(Query, cancellationToken);

            Items.Clear();
            foreach (NoteItem item in result)
            {
                Items.Add(item);
            }

            State = Items.Count == 0 ? NotesUiState.Empty : NotesUiState.Content;
        }
        catch (OperationCanceledException)
        {
            State = Items.Count == 0 ? NotesUiState.Idle : NotesUiState.Content;
            throw;
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
            State = NotesUiState.Error;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void SaveSelected()
    {
        if (Selected is null)
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(new NoteSavedMessage(Selected.Id));
    }

    private bool CanSave()
    {
        return Selected is not null && IsNotBusy;
    }

    partial void OnSelectedChanged(NoteItem? value)
    {
        SaveSelectedCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBusyChanged(bool value)
    {
        SaveSelectedCommand.NotifyCanExecuteChanged();
    }
}

public sealed class SaveReceipt : IRecipient<NoteSavedMessage>, IDisposable
{
    public Guid? LastSavedId { get; private set; }

    public SaveReceipt()
    {
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(NoteSavedMessage message)
    {
        LastSavedId = message.Value;
    }

    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
