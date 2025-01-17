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
            AttachBehaviors(dialog, _logger);
        };
        
        // Display the dialog and return the result
        return await DialogHelper.ShowDialogAsync(dialog.ShowDialog<string?>);
    }
    
    /// <summary>
    /// Attaches drag-and-drop behaviors to the dialog controls, enabling logging for drag-and-drop events.
    /// </summary>
    /// <param name="dialog">The dialog to which behaviors are attached.</param>
    /// <param name="logger">The logger used to log drag-and-drop events.</param>
    private static void AttachBehaviors(DragDropDialog dialog, IPluginLogger? logger)
    {
        logger?.Information("Attaching behaviors...");

        // Attach DragBehavior to ListBox items
        var listBox = dialog.FindControl<ListBox>("ItemListBox");
        if (listBox is not null)
        {
            logger?.Information("Found ListBox. Attaching DragBehavior...");
            for (var i = 0; i < listBox.ItemCount; i++)
            {
                var container = listBox.ContainerFromIndex(i);
                if (container is null)
                {
                    logger?.Warning($"No container found for ListBox item at index {i}. Retrying...");
                    continue;
                }

                var dragBehavior = new DragBehavior();
                dragBehavior.SetLogger(logger); // Set logger
                dragBehavior.DragData = listBox.Items.ElementAt(i); // Assign data context
                Interaction.GetBehaviors(container).Add(dragBehavior);
                logger?.Information($"DragBehavior attached to ListBox item at index {i}.");
            }
        }
        else
        {
            logger?.Warning("ListBox not found in dialog.");
        }

        // Attach DropBehavior to ItemsControl cells
        var itemsControl = dialog.FindControl<ItemsControl>("GridItemsControl");
        if (itemsControl is not null)
        {
            logger?.Information("Found ItemsControl. Attaching DropBehavior...");
            for (var i = 0; i < itemsControl.ItemCount; i++)
            {
                var container = itemsControl.ContainerFromIndex(i);
                if (container is null)
                {
                    logger?.Warning($"No container found for ItemsControl cell at index {i}. Retrying...");
                    continue;
                }

                var dropBehavior = new DropBehavior();
                dropBehavior.SetLogger(logger); // Set logger
                Interaction.GetBehaviors(container).Add(dropBehavior);
                logger?.Information($"DropBehavior attached to ItemsControl cell at index {i}.");
            }
        }
        else
        {
            logger?.Warning("ItemsControl not found in dialog.");
        }

        logger?.Information("Finished attaching behaviors.");
    }
}