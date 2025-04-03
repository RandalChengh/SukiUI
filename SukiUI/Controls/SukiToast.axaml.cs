using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using SukiUI.ColorTheme;
using SukiUI.Content;
using SukiUI.Toasts;

namespace SukiUI.Controls;

[TemplatePart("PART_ToastCard", typeof(Border))]
[TemplatePart("PART_DismissProgressBar", typeof(ProgressBar))]
public class SukiToast : ContentControl, ISukiToast
{
    private bool _wasDismissTimerInterrupted;
    //private readonly Stopwatch _dismissStopwatch = new();
    private Border? _toastCard;

    public ISukiToastManager? Manager { get; set; }
    public Action<ISukiToast, SukiToastDismissSource>? OnDismissed { get; set; }
    public Action<ISukiToast>? OnClicked { get; set; }

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<SukiToast, object?>(nameof(Icon));

    public static readonly DirectProperty<SukiToast, double> DismissStartTimestampProperty =
        AvaloniaProperty.RegisterDirect<SukiToast, double>(nameof(DismissStartTimestamp), o => o.DismissStartTimestamp,
            (o, value) => o.DismissStartTimestamp = value);

    private double _dismissStartTimestamp;
    public double DismissStartTimestamp
    {
        get => _dismissStartTimestamp;
        set => SetAndRaise(DismissStartTimestampProperty, ref _dismissStartTimestamp, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SukiToast, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<bool> LoadingStateProperty = AvaloniaProperty.Register<SukiToast, bool>(nameof(LoadingState));

    public bool LoadingState
    {
        get => GetValue(LoadingStateProperty);
        set => SetValue(LoadingStateProperty, value);
    }

    public static readonly StyledProperty<bool> CanDismissByClickingProperty = AvaloniaProperty.Register<SukiToast, bool>(nameof(CanDismissByClicking));

    public bool CanDismissByClicking
    {
        get => GetValue(CanDismissByClickingProperty);
        set => SetValue(CanDismissByClickingProperty, value);
    }

    public static readonly StyledProperty<bool> CanDismissByTimeProperty = AvaloniaProperty.Register<SukiToast, bool>(nameof(CanDismissByTime));

    public bool CanDismissByTime
    {
        get => GetValue(CanDismissByTimeProperty);
        set => SetValue(CanDismissByTimeProperty, value);
    }

    public static readonly DirectProperty<SukiToast, TimeSpan> DismissTimeoutProperty =
        AvaloniaProperty.RegisterDirect<SukiToast, TimeSpan>(nameof(DismissTimeout), o => o.DismissTimeout, (o, value) => o.DismissTimeout = value);

    private TimeSpan _dismissTimeout = TimeSpan.FromSeconds(5);
    public TimeSpan DismissTimeout
    {
        get => _dismissTimeout;
        set => SetAndRaise(DismissTimeoutProperty, ref _dismissTimeout, value);
    }

    public static readonly StyledProperty<bool> InterruptDismissTimerWhileHoverProperty =
        AvaloniaProperty.Register<SukiToast, bool>(nameof(InterruptDismissTimerWhileHover), true);

    public bool InterruptDismissTimerWhileHover
    {
        get => GetValue(InterruptDismissTimerWhileHoverProperty);
        set => SetValue(InterruptDismissTimerWhileHoverProperty, value);
    }

    public static readonly DirectProperty<SukiToast, double> DismissProgressValueProperty =
        AvaloniaProperty.RegisterDirect<SukiToast, double>(nameof(DismissProgressValue), o => o.DismissProgressValue,
            (o, value) => o.DismissProgressValue = value);

    private double _dismissProgressValue = 100;
    public double DismissProgressValue
    {
        get => _dismissProgressValue;
        set => SetAndRaise(DismissProgressValueProperty, ref _dismissProgressValue, value);
    }

    public static readonly StyledProperty<ObservableCollection<object>> ActionButtonsProperty = AvaloniaProperty.Register<SukiToast,
        ObservableCollection<object>>(nameof(ActionButtons), new ObservableCollection<object>());

    public ObservableCollection<object> ActionButtons
    {
        get => GetValue(ActionButtonsProperty);
        set => SetValue(ActionButtonsProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_toastCard is not null)
        {
            _toastCard.PointerPressed -= ToastCardClickedHandler;
        }

        _toastCard = e.NameScope.Get<Border>("PART_ToastCard");
        _toastCard.PointerPressed += ToastCardClickedHandler;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);


        DismissStartTimestamp = Stopwatch.GetTimestamp() * 1000d / Stopwatch.Frequency;
        //if (CanDismissByTime && _dismissTimer.Interval.TotalMilliseconds > 0)
        //{
        //    StartDismissTimer();
        //}
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        DismissStartTimestamp = 0;
        //StopDismissTimer();
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);

        if (InterruptDismissTimerWhileHover)
        {
            _wasDismissTimerInterrupted = true;
            DismissProgressValue = 100;
            DismissStartTimestamp = 0;
            //StopDismissTimer();
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);

        if (_wasDismissTimerInterrupted)
        {
            _wasDismissTimerInterrupted = false;
            if (IsLoaded && CanDismissByTime) // Need to check for IsLoaded as this method will still trigger after Unloaded!
                DismissStartTimestamp = Stopwatch.GetTimestamp() * 1000d / Stopwatch.Frequency;
        }
    }

    private void ToastCardClickedHandler(object o, PointerPressedEventArgs pointerPressedEventArgs)
    {
        OnClicked?.Invoke(this);
        if (!CanDismissByClicking) return;
        Manager.Dismiss(this, SukiToastDismissSource.Click);
        OnDismissed?.Invoke(this, SukiToastDismissSource.Click);
    }

    private void DismissTimerOnTick(object sender, EventArgs e)
    {
        //StopDismissTimer(0);
        Manager.Dismiss(this, SukiToastDismissSource.Timeout);
        OnDismissed?.Invoke(this, SukiToastDismissSource.Timeout);
    }

    private void DismissProgressValueTimerOnTick(object sender, EventArgs e)
    {
        //DismissProgressValue = Math.Min(Math.Max(100 - _dismissStopwatch.ElapsedMilliseconds / DismissTimeout.TotalMilliseconds * 100, 0), 100);
    }

    public void AnimateShow()
    {
        this.Animate(OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(500));
        this.Animate<double>(MaxHeightProperty, 0, 500, TimeSpan.FromMilliseconds(500));
        this.Animate(MarginProperty, new Thickness(0, 10, 0, -10), new Thickness(), TimeSpan.FromMilliseconds(500));
    }

    public void AnimateDismiss()
    {
        this.Animate(OpacityProperty, 1d, 0d, TimeSpan.FromMilliseconds(300));
        this.Animate(MarginProperty, new Thickness(), new Thickness(0, 0, 0, 10), TimeSpan.FromMilliseconds(300));
    }

    public ISukiToast ResetToDefault()
    {
        _wasDismissTimerInterrupted = false;
        DismissStartTimestamp = 0;

        Title = string.Empty;
        Content = string.Empty;
        Icon = Icons.InformationOutline;
        Foreground = NotificationColor.InfoIconForeground;
        CanDismissByClicking = false;
        CanDismissByTime = false;
        DismissTimeout = TimeSpan.FromSeconds(5);
        InterruptDismissTimerWhileHover = true;
        DismissProgressValue = 100;

        ActionButtons.Clear();
        OnDismissed = null;
        OnClicked = null;
        LoadingState = false;
        DockPanel.SetDock(this, Dock.Bottom);
        return this;
    }
}