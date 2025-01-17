using System.Collections.ObjectModel;
using ReactiveUI;

namespace ScriptRunner.Plugins.AzureDevOps.ViewModels;

/// <summary>
/// Represents the ViewModel for the Drag-and-Drop dialog.
/// </summary>
/// <remarks>
/// This ViewModel provides the data bindings for the drag-and-drop demo, including the list of items
/// to drag and the grid items where they can be dropped.
/// </remarks>
public class DragDropDialogModel : ReactiveObject
{
    /// <summary>
    /// Gets the collection of items available for dragging.
    /// </summary>
    /// <remarks>
    /// These items represent the source data that users can drag to the grid.
    /// </remarks>
    public ObservableCollection<string> Items { get; } =
    [
        "Item 1",
        "Item 2",
        "Item 3",
        "Item 4"
    ];

    /// <summary>
    /// Gets the collection of grid items where dragged items can be dropped.
    /// </summary>
    /// <remarks>
    /// The grid is represented as a 4x4 collection of strings. Each cell in the grid can hold a dropped item.
    /// </remarks>
    public ObservableCollection<string?> GridItems { get; set; } =
    [
        "Item A", "Item B", "Item C", "Item D",
        "Item E", "Item F", "Item G", "Item H",
        "Item I", "Item J", "Item K", "Item L",
        "Item M", "Item N", "Item O", "Item P"
    ];
}