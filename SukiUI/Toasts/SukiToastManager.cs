namespace SukiUI.Toasts;

// It's important that events are raised BEFORE removing them from the manager so that the animation only plays once.
public class SukiToastManager : ISukiToastManager
{
    public event SukiToastQueuedEventHandler? OnToastQueued;
    public event SukiToastDismissedEventHandler? OnToastDismissed;
    public event EventHandler? OnAllToastsDismissed;

    private readonly List<ISukiToast> _toasts = new();

    public void Queue(ISukiToast toast)
    {
        _toasts.Add(toast);
        OnToastQueued?.Invoke(this, new SukiToastQueuedEventArgs(toast));
    }

    public void Dismiss(ISukiToast toast, SukiToastDismissSource dismissSource = SukiToastDismissSource.Code)
    {
        if (!_toasts.Contains(toast)) return;
        OnToastDismissed?.Invoke(this, new SukiToastDismissedEventArgs(toast, dismissSource));
        _toasts.Remove(toast);
    }

    public void Dismiss(int count)
    {
        if (!_toasts.Any()) return;
        if (count > _toasts.Count) count = _toasts.Count;
        for (var i = 0; i < count; i++)
        {
            var removed = _toasts[i];
            OnToastDismissed?.Invoke(this, new SukiToastDismissedEventArgs(removed, SukiToastDismissSource.Code));
            _toasts.RemoveAt(i);
        }
    }

    public void EnsureMaximum(int maxAllowed)
    {
        if (_toasts.Count <= maxAllowed) return;
        Dismiss(_toasts.Count - maxAllowed);
    }

    public void DismissAll()
    {
        if (!_toasts.Any()) return;
        OnAllToastsDismissed?.Invoke(this, EventArgs.Empty);
        _toasts.Clear();
    }

    public bool IsDismissed(ISukiToast toast) => !_toasts.Contains(toast);
}