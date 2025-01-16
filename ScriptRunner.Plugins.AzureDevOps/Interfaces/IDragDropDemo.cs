using System.Threading.Tasks;

namespace ScriptRunner.Plugins.AzureDevOps.Interfaces;

/// <summary>
/// Represents a service for displaying a drag-and-drop demo interface.
/// </summary>
public interface IDragDropDemo
{
    /// <summary>
    /// Displays a drag-and-drop demo window.
    /// </summary>
    /// <param name="title">The title of the demo window. Defaults to "Drag &amp; Drop Demo".</param>
    /// <param name="width">The width of the demo window in pixels. Defaults to 800.</param>
    /// <param name="height">The height of the demo window in pixels. Defaults to 600.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The result is a string representing the data that was dropped during the demo, or <c>null</c> if no data was dropped.
    /// </returns>
    Task<string?> DisplayDragDropDemoAsync(
        string title = "Drag & Drop Demo",
        int width = 800,
        int height = 600);
}
