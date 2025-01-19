using System.Collections.ObjectModel;
using ReactiveUI;

namespace ScriptRunner.Plugins.AzureDevOps.ViewModels;

/// <summary>
///     Represents the ViewModel for the Drag-and-Drop dialog.
/// </summary>
/// <remarks>
///     This ViewModel provides the data bindings for the drag-and-drop demo, including the list of items
///     to drag and the grid items where they can be dropped.
/// </remarks>
public class DragDropDialogModel : ReactiveObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DragDropDialogModel" /> class.
    ///     Sets up the <see cref="GridItems" /> collection with 16 empty slots, representing a 4x4 grid.
    /// </summary>
    public DragDropDialogModel()
    {
        Items = ["Item 1", "Item 2", "Item 3", "Item 4"];
        // Initialize 4x4 grid with empty slots
        GridItems = new ObservableCollection<string?>(new string?[16]);
    }

    /// <summary>
    ///     Items
    /// </summary>
    public ObservableCollection<string> Items { get; set; }

    /// <summary>
    ///     GridItems
    /// </summary>
    public ObservableCollection<string?> GridItems { get; set; }
}