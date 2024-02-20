using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace PresentationAppWpf.Mvvm.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    private SnackbarMessageQueue _messageQueue = new(TimeSpan.FromSeconds(3));

    public SnackbarMessageQueue MessageQueue
    {
        get => _messageQueue;
        set => SetProperty(ref _messageQueue, value);
    }

    protected void ShowMessage(string message)
    {
        _messageQueue.Enqueue(message);
    }
}
