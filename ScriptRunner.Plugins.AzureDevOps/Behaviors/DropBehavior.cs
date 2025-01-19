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

        AssociatedObject?.RemoveHandler(DragDrop.DropEvent, OnDrop);
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
                // Get the index of the dropped container
                var index = GetIndexFromContainer(parentControl, AssociatedObject);

                if (index >= 0 && index < viewModel.GridItems.Count)
                {
                    viewModel.GridItems[index] = droppedText;
                }
                else
                {
                    _logger?.Warning($"Invalid index {index}. Cannot update GridItems.");
                }
            }
            else
            {
                _logger?.Warning("Parent control is not an ItemsControl or DataContext is invalid.");
            }

            DropCompleted?.Invoke(this, droppedText);
        }
        else
        {
            _logger?.Warning("Drop: invalid data format.");
        }
    }
    
    private static int GetIndexFromContainer(ItemsControl parentControl, Control? container)
    {
        while (container != null && container != parentControl)
        {
            // Use ItemsControl.IndexFromContainer directly to get the index
            var index = parentControl.IndexFromContainer(container);
            if (index >= 0)
            {
                return index;
            }

            // Ascend the tree to find the actual container
            container = container.Parent as Control;
        }
        return -1; // Not found
    }
}