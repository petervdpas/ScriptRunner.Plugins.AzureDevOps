using System;
using Avalonia.Controls;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
/// Provides a factory for creating behaviors with logging capabilities.
/// </summary>
public static class BehaviorFactory
{
    /// <summary>
    /// Creates an instance of a behavior with the specified logger.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the behavior to create. Must inherit from <see cref="BaseBehavior{Control}" /> and have a parameterless constructor.</typeparam>
    /// <param name="logger">The logger instance to associate with the behavior.</param>
    /// <returns>
    /// A new instance of the specified behavior type with the logger set.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is <c>null</c>.</exception>
    public static TBehavior Create<TBehavior>(IPluginLogger logger) where TBehavior : BaseBehavior<Control>, new()
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        var behavior = new TBehavior();
        behavior.SetLogger(logger);
        return behavior;
    }
}
