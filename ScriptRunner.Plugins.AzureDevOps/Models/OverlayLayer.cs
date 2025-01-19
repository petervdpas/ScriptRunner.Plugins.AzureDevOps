using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace ScriptRunner.Plugins.AzureDevOps.Models;

/// <summary>
///     Provides methods to manage and retrieve the overlay layer used for drag-and-drop ghosting.
/// </summary>
public static class OverlayLayer
{
    /// <summary>
    ///     Gets the overlay layer (a canvas) from the specified top-level visual root.
    /// </summary>
    /// <param name="topLevel">The top-level container, such as a Window.</param>
    /// <returns>The overlay layer as a <see cref="Canvas" /> if found; otherwise, <c>null</c>.</returns>
    public static Canvas? GetOverlayLayer(TopLevel topLevel)
    {
        // Use GetVisualChildren() to iterate over visual children safely
        return topLevel.GetVisualChildren()
            .OfType<Canvas>()
            .FirstOrDefault(child => child.Name == "OverlayLayer");
    }
}