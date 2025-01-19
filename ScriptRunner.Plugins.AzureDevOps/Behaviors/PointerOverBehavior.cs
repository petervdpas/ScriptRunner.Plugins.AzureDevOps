using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
///     A behavior to change the background color of a control (like a Border) when the pointer is over it.
/// </summary>
public class PointerOverBehavior : Behavior<Border>
{
    /// <summary>
    ///     The default color of the Border when the pointer is not over it.
    /// </summary>
    public static readonly StyledProperty<IBrush> DefaultColorProperty =
        AvaloniaProperty.Register<PointerOverBehavior, IBrush>(nameof(DefaultColor));

    /// <summary>
    ///     The color of the Border when the pointer is over it.
    /// </summary>
    public static readonly StyledProperty<IBrush> PointerOverColorProperty =
        AvaloniaProperty.Register<PointerOverBehavior, IBrush>(nameof(PointerOverColor));

    /// <summary>
    ///     Gets or sets the default color of the Border.
    /// </summary>
    public IBrush DefaultColor
    {
        get => GetValue(DefaultColorProperty);
        set => SetValue(DefaultColorProperty, value);
    }

    /// <summary>
    ///     Gets or sets the color of the Border when the pointer is over it.
    /// </summary>
    public IBrush PointerOverColor
    {
        get => GetValue(PointerOverColorProperty);
        set => SetValue(PointerOverColorProperty, value);
    }

    /// <summary>
    ///     Called when the behavior is attached to a control.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null) return;

        AssociatedObject.PointerEntered += OnPointerEnter;
        AssociatedObject.PointerExited += OnPointerLeave;

        // Set the initial background color
        AssociatedObject.Background = DefaultColor;
    }

    /// <summary>
    ///     Called when the behavior is detached from a control.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject == null) return;

        AssociatedObject.PointerEntered -= OnPointerEnter;
        AssociatedObject.PointerExited -= OnPointerLeave;
    }

    private void OnPointerEnter(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject != null) AssociatedObject.Background = PointerOverColor;
    }

    private void OnPointerLeave(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject != null) AssociatedObject.Background = DefaultColor;
    }
}