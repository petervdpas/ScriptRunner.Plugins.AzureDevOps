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
/// Implements the drag-and-drop demo interface, providing functionality to display a drag-and-drop demo window.
/// </summary>
public class DragDropDemo : IDragDropDemo
{
    private readonly IPluginLogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DragDropDemo"/> class with a logger.
    /// </summary>
    /// <param name="logger">
    /// An optional logger implementing <see cref="IPluginLogger"/> used to log drag-and-drop events.
    /// </param>
    public DragDropDemo(IPluginLogger? logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Displays a drag-and-drop demo dialog with the specified title, width, and height.
    /// </summary>
    /// <param name="title">The title of the dialog window. Defaults to "Drag &amp; Drop Demo".</param>
    /// <param name="width">The width of the dialog window in pixels. Defaults to 800.</param>
    /// <param name="height">The height of the dialog window in pixels. Defaults to 600.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The result is a string containing the data dropped into the dialog, or <c>null</c> if no data was dropped.
    /// </returns>
    public async Task<string?> DisplayDragDropDemoAsync(string title = "Drag & Drop Demo", int width = 800, int height = 600)
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
            AttachBehaviors(dialog);
        };
        
        // Display the dialog and return the result
        return await DialogHelper.ShowDialogAsync(dialog.ShowDialog<string?>);
    }
    
    /// <summary>
    /// Attaches drag-and-drop behaviors to the dialog controls.
    /// </summary>
    /// <param name="dialog">The dialog to configure.</param>
    private void AttachBehaviors(DragDropDialog dialog)
    {
        _logger?.Information("Attaching behaviors...");

        AttachDragBehavior(dialog, "ItemListBox");
        AttachDropBehavior(dialog, "GridItemsControl");

        _logger?.Information("Finished attaching behaviors.");
    }

    /// <summary>
    /// Attaches drag behavior to a list or items control.
    /// </summary>
    private void AttachDragBehavior(DragDropDialog dialog, string controlName)
    {
        var control = dialog.FindControl<ItemsControl>(controlName);

        if (control is null)
        {
            _logger?.Warning($"Control with name '{controlName}' not found.");
            return;
        }

        _logger?.Information($"Found {controlName}. Attaching DragBehavior...");

        for (var i = 0; i < control.ItemCount; i++)
        {
            var container = control.ContainerFromIndex(i);
            if (container is null)
            {
                _logger?.Warning($"No container found for item at index {i}.");
                continue;
            }

            var behavior = new DragBehavior
            {
                DragData = control.Items.ElementAt(i)
            };

            behavior.SetLogger(_logger);
            Interaction.GetBehaviors(container).Add(behavior);
        }
    }

    /// <summary>
    /// Attaches drop behavior to an item-control.
    /// </summary>
    private void AttachDropBehavior(DragDropDialog dialog, string controlName)
    {
        var control = dialog.FindControl<ItemsControl>(controlName);

        if (control is null)
        {
            _logger?.Warning($"Control with name '{controlName}' not found.");
            return;
        }

        _logger?.Information($"Found {controlName}. Attaching DropBehavior...");

        for (var i = 0; i < control.ItemCount; i++)
        {
            var container = control.ContainerFromIndex(i);
            if (container is null)
            {
                _logger?.Warning($"No container found for item at index {i}.");
                continue;
            }

            var behavior = new DropBehavior();
            behavior.SetLogger(_logger);

            // Attach the DropCompleted event handler
            behavior.DropCompleted += (s, droppedData) =>
            {
                _logger?.Information($"Drop completed on container at index {i} with data: {droppedData}");
                // Additional logic for handling the drop
            };

            Interaction.GetBehaviors(container).Add(behavior);
        }
    }
}