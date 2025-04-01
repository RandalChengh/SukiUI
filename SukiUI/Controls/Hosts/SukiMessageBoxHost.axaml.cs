using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using SukiUI.MessageBox;

namespace SukiUI.Controls;

[TemplatePart("PART_AlternativeHeaderGrid", typeof(Grid))]
[TemplatePart("PART_HeaderGrid", typeof(Grid))]
[TemplatePart("PART_Content", typeof(ScrollViewer))]
[TemplatePart("PART_FooterGrid", typeof(Grid))]
[TemplatePart("PART_LeftContentItems", typeof(ItemsControl))]
[TemplatePart("PART_ActionButtons", typeof(ItemsControl))]
public class SukiMessageBoxHost : HeaderedContentControl
{
    public static readonly StyledProperty<bool> UseAlternativeHeaderStyleProperty = AvaloniaProperty.Register<SukiMessageBoxHost, bool>(nameof(UseAlternativeHeaderStyle));

    /// <summary>
    /// Gets or sets a value indicating whether to use the alternative header style.
    /// </summary>
    public bool UseAlternativeHeaderStyle
    {
        get => GetValue(UseAlternativeHeaderStyleProperty);
        set => SetValue(UseAlternativeHeaderStyleProperty, value);
    }

    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<SukiMessageBoxHost, object?>(nameof(Icon));

    /// <summary>
    /// Gets or sets the icon content to display on the header.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="IconPreset"/> property.
    /// </summary>
    public static readonly StyledProperty<SukiMessageBoxIcons?> IconPresetProperty =
        AvaloniaProperty.Register<SukiMessageBoxHost, SukiMessageBoxIcons?>(nameof(IconPreset));

    /// <summary>
    /// Gets or sets the preset icon to display on the header.
    /// </summary>
    public SukiMessageBoxIcons? IconPreset
    {
        get => GetValue(IconPresetProperty);
        set => SetValue(IconPresetProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="IconPresetSize"/> property.
    /// </summary>
    public static readonly StyledProperty<double> IconPresetSizeProperty =
        AvaloniaProperty.Register<SukiMessageBoxHost, double>(nameof(IconPresetSize), 24);

    /// <summary>
    /// Gets or sets the size of the preset icon.
    /// </summary>
    public double IconPresetSize
    {
        get => GetValue(IconPresetSizeProperty);
        set => SetValue(IconPresetSizeProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="FooterLeftItemsSource"/> property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> FooterLeftItemsSourceProperty =
        AvaloniaProperty.Register<SukiMessageBoxHost, IEnumerable?>(nameof(FooterLeftItemsSource));

    /// <summary>
    /// Gets or sets the items source to display in the footer left of the message box
    /// </summary>
    public IEnumerable? FooterLeftItemsSource
    {
        get => GetValue(FooterLeftItemsSourceProperty);
        set => SetValue(FooterLeftItemsSourceProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="ActionButtonsSource"/> property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable<Button>?> ActionButtonsSourceProperty =
        AvaloniaProperty.Register<SukiMessageBoxHost, IEnumerable<Button>?>(nameof(ActionButtonsSource));

    /// <summary>
    /// Gets or sets the action buttons to display in bottom right of the message box.
    /// </summary>
    public IEnumerable<Button>? ActionButtonsSource
    {
        get => GetValue(ActionButtonsSourceProperty);
        set => SetValue(ActionButtonsSourceProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="ActionButtonsPreset"/> property.
    /// </summary>
    public static readonly StyledProperty<SukiMessageBoxButtons?> ActionButtonsPresetProperty =
        AvaloniaProperty.Register<SukiMessageBoxHost, SukiMessageBoxButtons?>(nameof(ActionButtonsPreset));

    /// <summary>
    /// Gets or sets the action buttons to display in bottom right of the message box.
    /// </summary>
    public SukiMessageBoxButtons? ActionButtonsPreset
    {
        get => GetValue(ActionButtonsPresetProperty);
        set => SetValue(ActionButtonsPresetProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (ReferenceEquals(e.Property, IconPresetProperty) ||
            ReferenceEquals(e.Property, IconPresetSizeProperty))
        {
            var preset = IconPreset;
            if (preset is null) return;

            Icon = SukiMessageBoxIconsFactory.CreateIcon(preset.Value, IconPresetSize);
        }
        else if (ReferenceEquals(e.Property, ActionButtonsPresetProperty))
        {
            var preset = ActionButtonsPreset;
            if (preset is null) return;

            ActionButtonsSource = preset switch
            {
                SukiMessageBoxButtons.OK => new[]
                {
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.OK)
                },
                SukiMessageBoxButtons.OKCancel =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.OK),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Cancel)
                ],
                SukiMessageBoxButtons.YesNo =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Yes),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.No)
                ],
                SukiMessageBoxButtons.YesNoCancel =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Yes),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.No),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Cancel)
                ],
                SukiMessageBoxButtons.YesIgnore =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Yes),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Ignore)
                ],
                SukiMessageBoxButtons.RetryCancel =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Retry),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Cancel)
                ],
                SukiMessageBoxButtons.RetryIgnoreAbort =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Retry),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Ignore),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Abort)
                ],
                SukiMessageBoxButtons.RetryContinueCancel =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Retry),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Continue),
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Cancel)
                ],
                SukiMessageBoxButtons.Close =>
                [
                    SukiMessageBoxButtonsFactory.CreateButton(SukiMessageBoxResult.Close)
                ],
                _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, null)
            };
        }
    }


}