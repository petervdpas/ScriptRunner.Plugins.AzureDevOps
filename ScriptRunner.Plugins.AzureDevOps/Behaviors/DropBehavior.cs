using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
/// A behavior that enables drop functionality for a control in Avalonia.
/// </summary>
public class DropBehavior : Behavior<Control>
{
    private IPluginLogger? _logger;
    
    /// <summary>
    /// Sets the logger for drag-and-drop events.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public void SetLogger(IPluginLogger? logger) => _logger = logger;
    
    /// <summary>
    /// Defines the <see cref="DroppedData"/> property that stores the data dropped onto the control.
    /// </summary>
    public static readonly StyledProperty<object?> DroppedDataProperty =
        AvaloniaProperty.Register<DropBehavior, object?>(nameof(DroppedData));

    /// <summary>
    /// Gets or sets the data dropped onto the control.
    /// </summary>
    public object? DroppedData
    {
        get => GetValue(DroppedDataProperty);
        set => SetValue(DroppedDataProperty, value);
    }

    /// <summary>
    /// An event that fires when a drop operation is successfully completed.
    /// </summary>
    public event EventHandler<object?>? DropCompleted;
    
    /// <summary>
    /// Called when the behavior is attached to a control.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null)
        {
            _logger?.Warning("DropBehavior: AssociatedObject is null.");
            return;
        }
        
        _logger?.Information($"DropBehavior attached to {AssociatedObject}");
        AssociatedObject.AddHandler(DragDrop.DragOverEvent, OnDragOver, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(DragDrop.DropEvent, OnDrop, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Called when the behavior is detached from a control.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject == null) return;
        
        _logger?.Information($"DropBehavior detached from {AssociatedObject}");
        AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
        AssociatedObject.RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        _logger?.Information($"DragOver on {AssociatedObject}");

        if (e.Data.Contains(DataFormats.Text))
        {
            _logger?.Information("DragOver: valid data detected.");
            e.DragEffects = DragDropEffects.Copy | DragDropEffects.Move;
            e.Handled = true;
        }
        else
        {
            _logger?.Warning("DragOver: no valid data detected.");
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        _logger?.Information($"Drop event triggered on {AssociatedObject}.");

        if (e.Data.Contains(DataFormats.Text))
        {
            DroppedData = e.Data.GetText();
            _logger?.Information($"Data dropped successfully: {DroppedData}");
            e.Handled = true;

            // Notify subscribers about the completed drop
            DropCompleted?.Invoke(this, DroppedData);
        }
        else
        {
            _logger?.Warning("Drop: no valid data found.");
            e.Handled = true; // Mark as handled to prevent bubbling
        }
    }
}