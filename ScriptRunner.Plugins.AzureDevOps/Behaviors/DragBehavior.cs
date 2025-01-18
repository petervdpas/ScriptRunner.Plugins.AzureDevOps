using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
/// A behavior that enables drag-and-drop functionality for a control in Avalonia.
/// </summary>
public class DragBehavior : Behavior<Control>
{
    private IPluginLogger? _logger;
    private double _originalOpacity;
    
    /// <summary>
    /// Sets the logger for drag-and-drop events.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public void SetLogger(IPluginLogger? logger) => _logger = logger;

    /// <summary>
    /// Defines the <see cref="DragData"/> property that stores the data to be dragged.
    /// </summary>
    public static readonly StyledProperty<object?> DragDataProperty =
        AvaloniaProperty.Register<DragBehavior, object?>(nameof(DragData));

    /// <summary>
    /// Gets or sets the data to be used for the drag operation.
    /// </summary>
    public object? DragData
    {
        get => GetValue(DragDataProperty);
        set => SetValue(DragDataProperty, value);
    }

    /// <summary>
    /// Called when the behavior is attached to a control.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null) return;
        
        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Called when the behavior is detached from a control.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject == null) return;
        
        AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
        AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
    }

    /// <summary>
    /// Handles the PointerPressed event to start the drag operation.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Pointer pressed event arguments.</param>
    private async Task OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DragData == null || AssociatedObject == null) return;

        try
        {
            _originalOpacity = AssociatedObject.Opacity;
            AssociatedObject.Opacity = 0.5;

            var dragDataObject = new DataObject();
            dragDataObject.Set(DataFormats.Text, DragData?.ToString() ?? string.Empty);
            _logger?.Information($"Data set for drag operation: {DragData}");

            await DragDrop.DoDragDrop(e, dragDataObject, DragDropEffects.Copy | DragDropEffects.Move);
        }
        finally
        {
            // Ensure the original opacity is restored, even if an exception occurs
            ResetOpacity();
        }
    }
    
    /// <summary>
    /// Handles the PointerReleased event to finalize the drag operation and clean up resources.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Pointer released event arguments.</param>
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _logger?.Information($"PointerReleased on {AssociatedObject}");
        ResetOpacity();
    }
    
    /// <summary>
    /// Resets the opacity of the associated object to its original value.
    /// </summary>
    private void ResetOpacity()
    {
        if (AssociatedObject == null) return;
        AssociatedObject.Opacity = _originalOpacity;
    }
}