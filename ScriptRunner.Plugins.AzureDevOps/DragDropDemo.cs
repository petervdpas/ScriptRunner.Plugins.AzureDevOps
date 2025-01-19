using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.AzureDevOps.Behaviors;
using ScriptRunner.Plugins.AzureDevOps.Dialogs;
using ScriptRunner.Plugins.AzureDevOps.Interfaces;
using ScriptRunner.Plugins.AzureDevOps.ViewModels;
using ScriptRunner.Plugins.Logging;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
///     Implements the drag-and-drop demo interface, providing functionality to display a drag-and-drop demo window.
/// </summary>
public class DragDropDemo : IDragDropDemo
{
    private readonly IPluginLogger? _logger;
    private readonly IDragDropService _dragDropService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DragDropDemo"/> class with a logger and drag-drop service.
    /// </summary>
    /// <param name="logger">An optional logger implementing <see cref="IPluginLogger"/> for drag-and-drop events.</param>
    /// <param name="dragDropService">A drag-and-drop service for attaching behaviors.</param>
    public DragDropDemo(IDragDropService dragDropService, IPluginLogger? logger)
    {
        _dragDropService = dragDropService ?? throw new ArgumentNullException(nameof(dragDropService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    ///     Displays a drag-and-drop demo dialog with the specified title, width, and height.
    /// </summary>
    /// <param name="title">The title of the dialog window. Defaults to "Drag &amp; Drop Demo".</param>
    /// <param name="width">The width of the dialog window in pixels. Defaults to 800.</param>
    /// <param name="height">The height of the dialog window in pixels. Defaults to 600.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous operation.
    ///     The result is a string containing the data dropped into the dialog, or <c>null</c> if no data was dropped.
    /// </returns>
    public async Task<string?> DisplayDragDropDemoAsync(string title = "Drag & Drop Demo", int width = 800,
        int height = 600)
    {
        // Create and configure the Azure DevOps dialog
        var dialog = new DragDropDialog
        {
            Title = title,
            Width = width,
            Height = height
        };

        // Set the data context to a new AzureDevOpsModel instance
        var viewModel = new DragDropDialogModel();
        dialog.DataContext = viewModel;

        // Log dialog initialization
        _logger?.Information($"Initializing DragDropDialog with title: {title}, width: {width}, height: {height}");

        // Attach behaviors once the dialog is opened
        dialog.Opened += async (_, _) =>
        {
            _logger?.Information("Dialog opened. Attaching behaviors after delay...");
            await Task.Delay(50); // Add a slight delay to ensure the layout is complete
            AttachBehaviors(dialog, viewModel);
        };

        // Display the dialog and return the result
        return await DialogHelper.ShowDialogAsync(dialog.ShowDialog<string?>);
    }

    /// <summary>
    /// Attaches drag-and-drop behaviors to the dialog controls.
    /// </summary>
    /// <param name="dialog">The dialog to configure.</param>
    /// <param name="viewModel">The ViewModel associated with the dialog.</param>
    private void AttachBehaviors(DragDropDialog dialog, DragDropDialogModel viewModel)
    {
        _logger?.Information("Attaching drag-and-drop behaviors...");

        // Attach drag behavior to the ItemListBox
        var itemListBox = dialog.FindControl<ItemsControl>("ItemListBox");
        _dragDropService.AttachBehavior<DragBehavior>(itemListBox, behavior =>
        {
            behavior.DragStartedAction = data =>
            {
                _logger?.Information($"Drag started with data: {data}");
            };
        });

        // Attach drop behavior to the GridItemsControl
        var gridItemsControl = dialog.FindControl<ItemsControl>("GridItemsControl");
        _dragDropService.AttachBehavior<DropBehavior>(gridItemsControl, behavior =>
        {
            behavior.DropAction = droppedData =>
            {
                _logger?.Information($"Drop action executed with data: {droppedData}");
                if (droppedData is not string text) return;
                
                // Find the index of the drop target
                var index = _dragDropService.GetIndexFromContainer(gridItemsControl, behavior.AssociatedObject);
                if (index >= 0 && index < viewModel.GridItems.Count)
                {
                    viewModel.GridItems[index] = text;
                    _logger?.Information($"GridItems updated at index {index} with data: {text}");
                }
                else
                {
                    _logger?.Warning("Invalid index for drop action.");
                }
            };
        });

        _logger?.Information("Behaviors attached successfully.");
    }
}