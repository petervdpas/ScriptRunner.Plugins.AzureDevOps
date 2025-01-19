using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.AzureDevOps.Behaviors;
using ScriptRunner.Plugins.AzureDevOps.Interfaces;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
/// A generic service for attaching behaviors to controls with customizable actions.
/// </summary>
public class DragDropService : IDragDropService
{
    private readonly IPluginLogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DragDropService"/> class.
    /// </summary>
    /// <param name="logger">An optional logger instance for events.</param>
    public DragDropService(IPluginLogger? logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Attaches a behavior of the specified type to an ItemsControl.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the behavior to attach.</typeparam>
    /// <param name="itemsControl">The ItemsControl to attach the behavior to.</param>
    /// <param name="configureBehavior">An optional action to configure each behavior instance.</param>
    public void AttachBehavior<TBehavior>(
        ItemsControl? itemsControl, 
        Action<TBehavior>? configureBehavior = null)
        where TBehavior : BaseBehavior<Control>, new()
    {
        if (itemsControl is null)
        {
            _logger?.Warning($"AttachBehavior<{typeof(TBehavior).Name}>: ItemsControl is null.");
            return;
        }

        _logger?.Information($"Attaching {typeof(TBehavior).Name} to ItemsControl: {itemsControl.Name}");

        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var container = itemsControl.ContainerFromIndex(i);
            if (container is null)
            {
                _logger?.Warning($"No container found for item at index {i}.");
                continue;
            }

            var behavior = BehaviorFactory.Create<TBehavior>(_logger!);
            configureBehavior?.Invoke(behavior);

            Interaction.GetBehaviors(container).Add(behavior);
        }
    }

    /// <summary>
    /// Attaches a behavior of the specified type to all child controls of a parent control.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the behavior to attach.</typeparam>
    /// <param name="parentControl">The parent control containing child controls to attach behaviors to.</param>
    /// <param name="childFilter">An optional filter to include only specific child controls.</param>
    /// <param name="configureBehavior">An optional action to configure each behavior instance.</param>
    public void AttachBehaviorToChildren<TBehavior>(
        Control? parentControl,
        Func<Control, bool>? childFilter = null,
        Action<TBehavior>? configureBehavior = null)
        where TBehavior : BaseBehavior<Control>, new()
    {
        if (parentControl is null)
        {
            _logger?.Warning($"AttachBehaviorToChildren<{typeof(TBehavior).Name}>: Parent control is null.");
            return;
        }

        _logger?.Information($"Attaching {typeof(TBehavior).Name} to children of: {parentControl.Name}");

        foreach (var child in parentControl.GetLogicalChildren().OfType<Control>())
        {
            if (childFilter?.Invoke(child) == false) continue;

            var behavior = BehaviorFactory.Create<TBehavior>(_logger!);
            configureBehavior?.Invoke(behavior);

            Interaction.GetBehaviors(child).Add(behavior);
        }
    }
    
    /// <summary>
    /// Gets the index of a container within an ItemsControl.
    /// </summary>
    /// <param name="parentControl">The parent ItemsControl.</param>
    /// <param name="container">The container whose index is to be determined.</param>
    /// <returns>
    /// The index of the container within the ItemsControl, or -1 if the container is not found.
    /// </returns>
    public int GetIndexFromContainer(ItemsControl? parentControl, Control? container)
    {
        while (container != null && container != parentControl)
        {
            if (parentControl != null)
            {
                var index = parentControl.IndexFromContainer(container);
                if (index >= 0) return index;
            }

            // Ascend the tree to find the actual container
            container = container.Parent as Control;
        }

        return -1; // Not found
    }
}