using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace ScriptRunner.Plugins.AzureDevOps.Models;

/// <summary>
/// Factory to create or manage dynamic content for cells.
/// </summary>
public static class ContentFactory
{
    /// <summary>
    /// Adds or updates content in a given container based on its current state.
    /// </summary>
    /// <param name="currentContent">The current content of the cell.</param>
    /// <param name="newContent">The new content to add.</param>
    /// <returns>The updated content to be set in the cell.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="newContent"/> is not of type <see cref="Control"/>.</exception>
    public static object CreateOrUpdateContent(object? currentContent, object newContent)
    {
        // Ensure the new content is a valid Control
        if (newContent is not Control newControl)
        {
            throw new ArgumentException("newContent must be of type Control", nameof(newContent));
        }

        return currentContent switch
        {
            null => newControl,
            TextBox textBox => AddToTextBox(textBox, newContent.ToString() ?? string.Empty),
            ListBox listBox => AddToListBox(listBox, newContent),
            ComboBox comboBox => AddToComboBox(comboBox, newContent),
            TabControl tabControl => AddToTabControl(tabControl, newControl),
            Canvas canvas => AddToCanvas(canvas, newControl),
            Grid grid => AddToGrid(grid, newControl),
            Panel panel => AddToPanel(panel, newControl), // General handling for Panel after specific cases
            ItemsControl itemsControl => AddToItemsControl(itemsControl, newControl), // General handling for ItemsControl
            _ => WrapInStackPanel(currentContent, newControl), // Fallback to wrapping in a StackPanel
        };
    }

    /// <summary>
    /// Adds a new control to a panel.
    /// </summary>
    /// <param name="panel">The panel to add the new control to.</param>
    /// <param name="newControl">The new control to add.</param>
    /// <returns>The updated panel.</returns>
    private static Panel AddToPanel(Panel panel, Control newControl)
    {
        panel.Children.Add(newControl);
        return panel;
    }

    /// <summary>
    /// Adds a new control to an ItemsControl's item source.
    /// </summary>
    /// <param name="itemsControl">The ItemsControl to update.</param>
    /// <param name="newControl">The new control to add.</param>
    /// <returns>The updated ItemsControl.</returns>
    private static ItemsControl AddToItemsControl(ItemsControl itemsControl, Control newControl)
    {
        if (itemsControl.ItemsSource is not ObservableCollection<object> items)
        {
            items = [];
            itemsControl.ItemsSource = items;
        }

        items.Add(newControl);
        return itemsControl;
    }

    /// <summary>
    /// Wraps the existing content and the new control in a StackPanel.
    /// </summary>
    /// <param name="currentContent">The existing content to wrap.</param>
    /// <param name="newControl">The new control to add.</param>
    /// <returns>A StackPanel containing the existing and new content.</returns>
    private static StackPanel WrapInStackPanel(object currentContent, Control newControl)
    {
        var stackPanel = new StackPanel();

        if (currentContent is Control existingControl)
        {
            stackPanel.Children.Add(existingControl);
        }

        stackPanel.Children.Add(newControl);
        return stackPanel;
    }

    /// <summary>
    /// Adds new content to a TextBox as text.
    /// </summary>
    /// <param name="textBox">The TextBox to update.</param>
    /// <param name="newContent">The content to append to the TextBox.</param>
    /// <returns>The updated TextBox.</returns>
    private static TextBox AddToTextBox(TextBox textBox, string newContent)
    {
        textBox.Text += $"{newContent}\n";
        return textBox;
    }

    /// <summary>
    /// Adds new content to a ListBox.
    /// </summary>
    /// <param name="listBox">The ListBox to update.</param>
    /// <param name="newContent">The content to add to the ListBox.</param>
    /// <returns>The updated ListBox.</returns>
    private static ListBox AddToListBox(ListBox listBox, object newContent)
    {
        if (listBox.ItemsSource is not ObservableCollection<object> items)
        {
            items = [];
            listBox.ItemsSource = items;
        }

        items.Add(newContent);
        return listBox;
    }

    /// <summary>
    /// Adds new content to a ComboBox.
    /// </summary>
    /// <param name="comboBox">The ComboBox to update.</param>
    /// <param name="newContent">The content to add to the ComboBox.</param>
    /// <returns>The updated ComboBox.</returns>
    private static ComboBox AddToComboBox(ComboBox comboBox, object newContent)
    {
        if (comboBox.ItemsSource is not ObservableCollection<object> items)
        {
            items = [];
            comboBox.ItemsSource = items;
        }

        items.Add(newContent);
        return comboBox;
    }

    /// <summary>
    /// Adds a new control to a Grid.
    /// </summary>
    /// <param name="grid">The Grid to update.</param>
    /// <param name="newControl">The new control to add.</param>
    /// <returns>The updated Grid.</returns>
    private static Grid AddToGrid(Grid grid, Control newControl)
    {
        var rowCount = grid.RowDefinitions.Count;
        var colCount = grid.ColumnDefinitions.Count;

        var row = rowCount > 0 ? rowCount - 1 : 0;
        var col = colCount > 0 ? colCount - 1 : 0;

        Grid.SetRow(newControl, row);
        Grid.SetColumn(newControl, col);

        grid.Children.Add(newControl);
        return grid;
    }

    /// <summary>
    /// Adds a new tab to a TabControl.
    /// </summary>
    /// <param name="tabControl">The TabControl to update.</param>
    /// <param name="newControl">The control to set as the tab's content.</param>
    /// <param name="header">The header text for the new tab.</param>
    /// <returns>The updated TabControl.</returns>
    private static TabControl AddToTabControl(TabControl tabControl, Control newControl, string header = "New Tab")
    {
        var tabItem = new TabItem
        {
            Header = header,
            Content = newControl
        };

        if (tabControl.ItemsSource is not ObservableCollection<TabItem> tabItems)
        {
            tabItems = [];
            tabControl.ItemsSource = tabItems;
        }

        tabItems.Add(tabItem);
        return tabControl;
    }

    /// <summary>
    /// Adds a new control to a Canvas at the specified position.
    /// </summary>
    /// <param name="canvas">The Canvas to update.</param>
    /// <param name="newControl">The new control to add.</param>
    /// <param name="left">The left position for the new control.</param>
    /// <param name="top">The top position for the new control.</param>
    /// <returns>The updated Canvas.</returns>
    private static Canvas AddToCanvas(Canvas canvas, Control newControl, double left = 0, double top = 0)
    {
        Canvas.SetLeft(newControl, left);
        Canvas.SetTop(newControl, top);
        canvas.Children.Add(newControl);
        return canvas;
    }
}