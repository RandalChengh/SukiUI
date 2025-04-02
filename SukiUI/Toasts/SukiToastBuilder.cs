using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using SukiUI.ColorTheme;
using SukiUI.Content;
using SukiUI.Helpers;

namespace SukiUI.Toasts
{
    public class SukiToastBuilder
    {
        public ISukiToastManager Manager { get; }
        public ISukiToast Toast { get; }

        public SukiToastBuilder(ISukiToastManager manager)
        {
            Manager = manager;
            Toast = ToastPool.Get();
            Toast.Manager = Manager;
        }

        public ISukiToast Queue()
        {
            Manager.Queue(Toast);
            return Toast;
        }

        public void SetTitle(string title) => Toast.Title = title;

        public void SetContent(object? content) => Toast.Content = content;

        public void SetCanDismissByClicking(bool canDismiss) => Toast.CanDismissByClicking = canDismiss;
        public void SetLoadingState(bool loading) => Toast.LoadingState = loading;

        public void SetType(NotificationType type)
        {
            Toast.Icon = type switch
            {
                NotificationType.Information => Icons.InformationOutline,
                NotificationType.Success => Icons.Check,
                NotificationType.Warning => Icons.AlertOutline,
                NotificationType.Error => Icons.AlertOutline,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            Toast.Foreground = type switch
            {
                NotificationType.Information => NotificationColor.InfoIconForeground,
                NotificationType.Success => NotificationColor.SuccessIconForeground,
                NotificationType.Warning => NotificationColor.WarningIconForeground,
                NotificationType.Error => NotificationColor.ErrorIconForeground,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }


        public void SetDismissAfter(TimeSpan delay, bool interruptWhileHover = true)
        {
            Toast.InterruptDismissTimerWhileHover = interruptWhileHover;
            Toast.CanDismissByTime = delay.TotalMilliseconds > 0;
            Toast.DismissTimeout = delay;
        }

        public void SetOnDismiss(Action<ISukiToast, SukiToastDismissSource> action) => Toast.OnDismissed = action;

        public void SetOnClicked(Action<ISukiToast> action) => Toast.OnClicked = action;

        public void AddActionButton(object buttonContent, Action<ISukiToast> action, bool dismissOnClick, bool flatstyle = true)
        {
            Button btn = new Button()
                {
                    Content = buttonContent,
                    Classes = { flatstyle ?"Flat" : "Basic" },
                    Margin = flatstyle ? new Thickness(14, 9, 0, 12) : new Thickness(14, -3, 0, 2)
                };



            btn.Click += (_, _) =>
            {
                action(Toast);
                if(dismissOnClick)
                    Manager.Dismiss(Toast);
            };
            Toast.ActionButtons.Add(btn);
        }

        public class DismissToast
        {
            public SukiToastBuilder Builder { get; }

            public DismissToast(SukiToastBuilder builder)
            {
                Builder = builder;
            }
        }
    }
}