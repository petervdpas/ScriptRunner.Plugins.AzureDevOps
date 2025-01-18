using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
/// A behavior that enables drag-and-drop functionality for a control in Avalonia.
/// </summary>
public class DragBehavior : Behavior<Control>
{
    private IPluginLogger? _logger;
    
    private Control? _ghost; // The visual representation of the dragged item
    private Point _dragStartPoint;
    private bool _isDragging;
    private bool _dragInitiated;
    private double _originalOpacity;
    private const double DragThreshold = 5.0;
    
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
        
        _logger?.Information($"DragBehavior attached to {AssociatedObject}");
        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Called when the behavior is detached from a control.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject == null) return;
        
        _logger?.Information($"DragBehavior detached from {AssociatedObject}");
        AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
        AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
        AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
    }

    /// <summary>
    /// Handles the PointerPressed event to start the drag operation.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Pointer pressed event arguments.</param>
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _logger?.Information($"PointerPressed on {AssociatedObject}");
        
        if (DragData == null || AssociatedObject == null) return;

        _dragStartPoint = e.GetPosition(null);
        _isDragging = true;
        _dragInitiated = false;
    }
    
    /// <summary>
    /// Handles the PointerMoved event to initiate drag-and-drop or update the ghost position.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Pointer moved event arguments.</param>
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || AssociatedObject == null) return;

        var currentPosition = e.GetPosition(null);
        
        // Calculate distance moved
        var dx = currentPosition.X - _dragStartPoint.X;
        var dy = currentPosition.Y - _dragStartPoint.Y;
        var distance = Math.Sqrt(dx * dx + dy * dy);

        // If we haven't started dragging yet, and we've moved past the threshold
        if (!_dragInitiated && distance > DragThreshold)
        {
            _dragInitiated = true;
            CreateGhost(currentPosition);
            
            /*
            var dragDataString = DragData?.ToString() ?? string.Empty;
            var data = new DataObject();
            data.Set(DataFormats.Text, dragDataString);
            
            DragDrop.DoDragDrop(e, data, DragDropEffects.Copy | DragDropEffects.Move);
            */
        }
        
        // Update ghost position if it exists
        if (_ghost == null) return;
        
        Canvas.SetLeft(_ghost, currentPosition.X - _ghost.Bounds.Width / 2);
        Canvas.SetTop(_ghost, currentPosition.Y - _ghost.Bounds.Height / 2);
    }
    
    /// <summary>
    /// Handles the PointerReleased event to finalize the drag operation and clean up resources.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Pointer released event arguments.</param>
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isDragging) return;
        
        _logger?.Information("PointerReleased on AssociatedObject.");
        _isDragging = false;
        _dragInitiated = false;
        RemoveGhost();
    }
    
    /// <summary>
    /// Creates a visual ghost representation of the dragged control.
    /// </summary>
    /// <param name="pointerPosition">The position of the pointer.</param>
    private void CreateGhost(Point pointerPosition)
    {
        if (_ghost != null || AssociatedObject == null) return;

        _ghost = CloneControl(AssociatedObject);
        _ghost.Opacity = 0.7;
        _ghost.IsHitTestVisible = false;

        _originalOpacity = AssociatedObject.Opacity;
        AssociatedObject.Opacity = 0.5;
        
        var overlayLayer = (AssociatedObject.GetVisualRoot() as TopLevel)?
            .Find<Canvas>("OverlayLayer");

        if (overlayLayer == null)
        {
            _ghost = null;
            return;
        }

        overlayLayer.Children.Add(_ghost);
        Canvas.SetLeft(_ghost, pointerPosition.X - _ghost.Bounds.Width / 2);
        Canvas.SetTop(_ghost, pointerPosition.Y - _ghost.Bounds.Height / 2);
    }

    /// <summary>
    /// Removes the ghost control from the visual tree.
    /// </summary>
    private void RemoveGhost()
    {
        if (_ghost == null) return;

        if (AssociatedObject != null)
        {
            AssociatedObject.Opacity = _originalOpacity;
        }
        
        var overlayLayer = (AssociatedObject?.GetVisualRoot() as TopLevel)?
            .Find<Canvas>("OverlayLayer");

        if (overlayLayer?.Children.Contains(_ghost) == true)
        {
            overlayLayer.Children.Remove(_ghost);
        }

        _ghost = null;
    }
    
    /// <summary>
    /// Creates a shallow visual clone of the specified control.
    /// </summary>
    /// <param name="control">The control to clone.</param>
    /// <returns>A cloned instance of the control.</returns>
    private static ContentControl CloneControl(Control control)
    {
        // Create a new ContentControl to hold the cloned data
        var clone = new ContentControl
        {
            Width = control.Bounds.Width,
            Height = control.Bounds.Height,
            Background = new SolidColorBrush(Colors.LightGray, 0.7),
            Content = control.DataContext, // Bind the same data context
            ContentTemplate = (control as ContentControl)?.ContentTemplate, // Reuse template if applicable
            Margin = control.Margin
        };

        // Copy styles by adding each style to the clone's Styles collection
        foreach (var style in control.Styles)
        {
            clone.Styles.Add(style);
        }

        // Copy only non-pseudo-class items from Classes
        foreach (var className in control.Classes.Where(className => !className.StartsWith(':')))
        {
            clone.Classes.Add(className);
        }

        return clone;
    }
}