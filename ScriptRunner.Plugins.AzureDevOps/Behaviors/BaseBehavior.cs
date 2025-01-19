using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
/// A base class for behaviors that require a logger.
/// </summary>
/// <typeparam name="T">The type of the associated object.</typeparam>
public abstract class BaseBehavior<T> : Behavior<T> where T : Avalonia.Controls.Control
{
    /// <summary>
    /// Gets or sets the logger for the behavior.
    /// </summary>
    protected IPluginLogger? Logger { get; private set; }

    /// <summary>
    /// Sets the logger for this behavior.
    /// </summary>
    /// <param name="logger">The logger instance to set.</param>
    public void SetLogger(IPluginLogger logger)
    {
        Logger = logger;
    }
}