using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.AzureDevOps.ViewModels;
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
        
        AssociatedObject.SetValue(DragDrop.AllowDropProperty, true);
        
        AssociatedObject.AddHandler(
            DragDrop.DragOverEvent, 
            OnDragOver, 
            RoutingStrategies.Bubble);
        AssociatedObject.AddHandler(
            DragDrop.DropEvent, 
            OnDrop, 
            RoutingStrategies.Bubble);
    }

    /// <summary>
    /// Called when the behavior is detached from a control.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject == null) return;
        
        AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
        AssociatedObject.RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        _logger?.Information($"DragOver on {AssociatedObject}");

        if (e.Data.Contains(DataFormats.Text))
        {
            e.DragEffects = DragDropEffects.Copy | DragDropEffects.Move;
            e.Handled = true;

            // Optional: Highlight the drop zone
            if (AssociatedObject is Panel panel)
            {
                panel.Background = Brushes.LightBlue; // Example of visual feedback
            }
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Text))
        {
            var droppedText = e.Data.GetText();
            if (string.IsNullOrEmpty(droppedText))
            {
                _logger?.Warning("Dropped data is empty.");
                return;
            }

            if (AssociatedObject?.Parent is ItemsControl
                {
                    DataContext: DragDropDialogModel viewModel
                } parentControl)
            {
                var index = parentControl.IndexFromContainer(AssociatedObject);
                if (index >= 0 && index < viewModel.GridItems.Count)
                {
                    viewModel.GridItems[index] = droppedText;
                }
            }

            // Raise the event after successfully handling the drop
            DropCompleted?.Invoke(this, droppedText);
        }
        else
        {
            _logger?.Warning("Drop: invalid data format.");
        }

        // Reset background
        if (AssociatedObject is Panel panel)
        {
            panel.Background = Brushes.White;
        }
    }
    
}