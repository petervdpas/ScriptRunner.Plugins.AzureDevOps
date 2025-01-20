using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
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
    /// Gets the command to execute when a drag operation starts.
    /// </summary>
    public ICommand DragStartedCommand { get; }

    /// <summary>
    /// Gets the command to execute when a drop operation is completed.
    /// </summary>
    public ICommand DropCompletedCommand { get; }
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="DragDropDialogModel" /> class.
    ///     Sets up the <see cref="GridItems" /> collection with 16 empty slots, representing a 4x4 grid.
    /// </summary>
    public DragDropDialogModel()
    {
        Items = ["Item 1", "Item 2", "Item 3", "Item 4"];
        // Initialize 4x4 grid with empty cells
        GridItems = new ObservableCollection<GridCellViewModel>(
            Enumerable.Range(0, 16).Select(_ => new GridCellViewModel())
        );
        
        DragStartedCommand = ReactiveCommand.Create<object?>(OnDragStarted);
        DropCompletedCommand = ReactiveCommand.Create<object?>(OnDropCompleted);
    }

    /// <summary>
    ///     Items
    /// </summary>
    public ObservableCollection<string> Items { get; set; }

    /// <summary>
    /// Gets the list of grid cells.
    /// </summary>
    public ObservableCollection<GridCellViewModel> GridItems { get; set; }
    
    /// <summary>
    /// Called when a drag operation starts.
    /// </summary>
    /// <param name="dragData">The data being dragged.</param>
    private static void OnDragStarted(object? dragData)
    {
        // Log or process drag started event
        // Replace with application-specific logic
        System.Diagnostics.Debug.WriteLine($"Drag started with data: {dragData}");
    }

    /// <summary>
    /// Called when a drop operation is completed.
    /// </summary>
    /// <param name="droppedData">The data that was dropped.</param>
    private static void OnDropCompleted(object? droppedData)
    {
        // Log or process drop completed event
        // Replace with application-specific logic
        System.Diagnostics.Debug.WriteLine($"Drop completed with data: {droppedData}");
    }
}