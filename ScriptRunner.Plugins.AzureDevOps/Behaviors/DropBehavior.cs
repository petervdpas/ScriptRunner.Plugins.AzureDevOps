using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
///     A behavior that enables drop functionality for a control in Avalonia.
/// </summary>
public class DropBehavior : Behavior<Control>
{
    private IPluginLogger? _logger;
        
    /// <summary>
    ///     Defines the
    ///     <see>
    /// <cref>DroppedData</cref>
    ///     </see>
    ///     property that stores the data dropped onto the control.
    /// </summary>
    public static readonly StyledProperty<object?> DroppedDataProperty =
        AvaloniaProperty.Register<DropBehavior, object?>(nameof(DroppedData));

    /// <summary>
    /// Defines the <see cref="DropAction"/> property, which specifies an action to execute when data is dropped.
    /// </summary>
    public static readonly StyledProperty<Action<object?>?> DropActionProperty =
        AvaloniaProperty.Register<DropBehavior, Action<object?>?>(nameof(DropAction));

    /// <summary>
    ///     Gets or sets the data dropped onto the control.
    /// </summary>
    public object? DroppedData
    {
        get => GetValue(DroppedDataProperty);
        set => SetValue(DroppedDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the action to be executed when data is dropped.
    /// </summary>
    public Action<object?>? DropAction
    {
        get => GetValue(DropActionProperty);
        set => SetValue(DropActionProperty, value);
    }
    
    /// <summary>
    ///     Sets the logger for drag-and-drop events.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public void SetLogger(IPluginLogger? logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     An event that fires when a drop operation is successfully completed.
    /// </summary>
    public event EventHandler<object?>? DropCompleted;

    /// <summary>
    /// Called when the behavior is attached to a control.
    /// This method enables drag-and-drop functionality by setting the <see cref="DragDrop.AllowDropProperty"/> 
    /// and registering the <see cref="DragDrop.DropEvent"/> handler.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null)
        {
            _logger?.Warning("DropBehavior: AssociatedObject is null.");
            return;
        }

        AssociatedObject.SetValue(DragDrop.AllowDropProperty, true);
        AssociatedObject.AddHandler(
            DragDrop.DropEvent,
            OnDrop,
            RoutingStrategies.Bubble);
    }

    /// <summary>
    /// Called when the behavior is detached from a control.
    /// This method removes the <see cref="DragDrop.DropEvent"/> handler to clean up resources.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject?.RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    /// <summary>
    /// Handles the <see cref="DragDrop.DropEvent"/> to process dropped data and execute the specified action.
    /// </summary>
    /// <param name="sender">The control where the drop occurred.</param>
    /// <param name="e">Event data containing information about the drop operation.</param>
    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Text)) return;
        
        var droppedText = e.Data.GetText();
        DroppedData = droppedText;

        if (string.IsNullOrEmpty(droppedText)) return;
        
        DropAction?.Invoke(droppedText);

        // Invoke the DropCompleted event
        DropCompleted?.Invoke(this, droppedText);
    }
}