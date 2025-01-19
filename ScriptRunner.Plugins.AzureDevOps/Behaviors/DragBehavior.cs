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
/// A behavior that enables drag-and-drop functionality for a control in Avalonia.
/// </summary>
public class DragBehavior : Behavior<Control>
{
    private IPluginLogger? _logger;
    private double _originalOpacity;
    private bool _isDragging;

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
    
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DragData == null || AssociatedObject == null) return;

        // Fire the async task without awaiting it directly to avoid issues
        _ = StartDragDropAsync(e);
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ResetOpacity();
    }
    
    private void ResetOpacity()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.Opacity = _originalOpacity;
        }
    }
    
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