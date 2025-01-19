using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
///     A behavior that enables drag-and-drop functionality for a control in Avalonia.
/// </summary>
public class DragBehavior : Behavior<Control>
{
    private IPluginLogger? _logger;
    private double _originalOpacity;
    private bool _isDragging;

    /// <summary>
    ///     Defines the <see cref="DragData" /> property that stores the data to be dragged.
    /// </summary>
    public static readonly StyledProperty<object?> DragDataProperty =
        AvaloniaProperty.Register<DragBehavior, object?>(nameof(DragData));
    
    /// <summary>
    /// Defines the <see cref="DragStartedAction"/> property,
    /// which specifies an action to execute when a drag operation starts.
    /// </summary>
    public static readonly StyledProperty<Action<object?>?> DragStartedActionProperty =
        AvaloniaProperty.Register<DragBehavior, Action<object?>?>(nameof(DragStartedAction));
    
    /// <summary>
    ///     Gets or sets the data to be used for the drag operation.
    /// </summary>
    public object? DragData
    {
        get => GetValue(DragDataProperty);
        set => SetValue(DragDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the action to be executed when a drag operation starts.
    /// </summary>
    public Action<object?>? DragStartedAction
    {
        get => GetValue(DragStartedActionProperty);
        set => SetValue(DragStartedActionProperty, value);
    }
    
    /// <summary>
    ///     Sets the logger for drag-and-drop events.
    /// </summary>
    /// <param name="logger">The logger instance used to record events and errors.</param>
    public void SetLogger(IPluginLogger? logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when the behavior is attached to a control.
    /// This method registers event handlers for pointer press and release events to enable drag functionality.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null) return;

        AssociatedObject.AddHandler(
            InputElement.PointerPressedEvent,
            OnPointerPressed,
            RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(
            InputElement.PointerReleasedEvent,
            OnPointerReleased,
            RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Called when the behavior is detached from a control.
    /// This method unregisters the event handlers to clean up resources.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject == null) return;

        AssociatedObject.RemoveHandler(
            InputElement.PointerPressedEvent,
            OnPointerPressed);
        AssociatedObject.RemoveHandler(
            InputElement.PointerReleasedEvent,
            OnPointerReleased);
    }

    /// <summary>
    /// Handles the PointerPressed event to initiate the drag operation.
    /// </summary>
    /// <param name="sender">The sender of the event, typically the associated control.</param>
    /// <param name="e">Pointer pressed event arguments.</param>
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DragData == null || AssociatedObject == null) return;

        DragStartedAction?.Invoke(DragData);

        // Fire the async task without awaiting it directly to avoid issues
        _ = StartDragDropAsync(e);
    }

    /// <summary>
    /// Handles the PointerReleased event to reset the control's opacity after a drag operation.
    /// </summary>
    /// <param name="sender">The sender of the event, typically the associated control.</param>
    /// <param name="e">Pointer released event arguments.</param>
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ResetOpacity();
    }

    /// <summary>
    /// Resets the opacity of the associated control to its original value.
    /// </summary>
    private void ResetOpacity()
    {
        if (AssociatedObject != null) AssociatedObject.Opacity = _originalOpacity;
    }

    /// <summary>
    /// Asynchronously starts the drag-and-drop operation.
    /// </summary>
    /// <param name="eventArgs">Pointer pressed event arguments used to initiate the drag operation.</param>
    private async Task StartDragDropAsync(PointerPressedEventArgs eventArgs)
    {
        if (_isDragging) return;
        _isDragging = true;

        try
        {
            if (AssociatedObject != null)
            {
                _originalOpacity = AssociatedObject.Opacity;
                AssociatedObject.Opacity = 0.5;
            }

            var dragDataObject = new DataObject();
            dragDataObject.Set(
                DataFormats.Text,
                DragData?.ToString() ?? string.Empty);

            await DragDrop.DoDragDrop(
                eventArgs,
                dragDataObject,
                DragDropEffects.Copy | DragDropEffects.Move);
        }
        catch (Exception ex)
        {
            _logger?.Error($"Exception during drag-drop operation: {ex.Message}", ex);
        }
        finally
        {
            ResetOpacity();
            _isDragging = false;
        }
    }
}