using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.AzureDevOps.Behaviors;

namespace ScriptRunner.Plugins.AzureDevOps.Interfaces;

/// <summary>
/// Defines an interface for attaching behaviors to controls in Avalonia, supporting drag-and-drop operations or other interactions.
/// </summary>
public interface IDragDropService
{
    /// <summary>
    /// Attaches a behavior of the specified type to an <see cref="ItemsControl"/>. 
    /// Behaviors can modify or extend the functionality of controls in a reusable manner.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the behavior to attach, inheriting from <see cref="Behavior{T}"/>.</typeparam>
    /// <param name="itemsControl">
    /// The <see cref="ItemsControl"/> to which the behaviors will be attached.
    /// For example, an <see cref="ListBox"/> or a custom control.
    /// </param>
    /// <param name="configureBehavior">
    /// An optional action to configure each instance of the behavior before it is attached.
    /// For instance, you can provide specific actions to execute when the behavior triggers.
    /// </param>
    void AttachBehavior<TBehavior>(ItemsControl? itemsControl, Action<TBehavior>? configureBehavior = null)
        where TBehavior : BaseBehavior<Control>, new();

    /// <summary>
    /// Attaches a behavior of the specified type to all child controls of a parent control.
    /// This allows applying behaviors to nest elements dynamically.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the behavior to attach, inheriting from <see cref="Behavior{T}"/>.</typeparam>
    /// <param name="parentControl">
    /// The parent <see cref="Control"/> containing child controls to which the behaviors will be attached.
    /// This could be a <see cref="Panel"/>, <see cref="Grid"/>, or any control with logical children.
    /// </param>
    /// <param name="childFilter">
    /// An optional filter function to include or exclude specific child controls from having the behavior attached.
    /// This is useful for applying behaviors selectively to certain controls.
    /// </param>
    /// <param name="configureBehavior">
    /// An optional action to configure each instance of the behavior before it is attached.
    /// For example, you can provide initialization logic or parameters for the behavior.
    /// </param>
    void AttachBehaviorToChildren<TBehavior>(
        Control? parentControl,
        Func<Control, bool>? childFilter = null,
        Action<TBehavior>? configureBehavior = null)
        where TBehavior : BaseBehavior<Control>, new();

    /// <summary>
    /// Gets the index of a container within an ItemsControl.
    /// </summary>
    /// <param name="parentControl">The parent ItemsControl.</param>
    /// <param name="container">The container whose index is to be determined.</param>
    /// <returns>
    /// The index of the container within the ItemsControl, or -1 if the container is not found.
    /// </returns>
    int GetIndexFromContainer(ItemsControl? parentControl, Control? container);
}