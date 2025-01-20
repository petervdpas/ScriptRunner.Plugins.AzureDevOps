using ReactiveUI;
using ScriptRunner.Plugins.AzureDevOps.Models;

namespace ScriptRunner.Plugins.AzureDevOps.ViewModels;

/// <summary>
/// Represents a cell in the drag-and-drop grid.
/// </summary>
public class GridCellViewModel : ReactiveObject
{
    private object? _currentContent;

    /// <summary>
    /// Gets or sets the dynamic content of the cell.
    /// </summary>
    public object? CurrentContent
    {
        get => _currentContent;
        set => this.RaiseAndSetIfChanged(ref _currentContent, value);
    }
    
    /// <summary>
    /// Adds new content to the cell using the ContentFactory.
    /// </summary>
    /// <param name="newContent">The new content to add.</param>
    public void AddContent(object newContent)
    {
        CurrentContent = ContentFactory.CreateOrUpdateContent(CurrentContent, newContent);
    }
    
    /// <summary>
    /// Removes the content for the cell.
    /// </summary>
    public void RemoveContent()
    {
        CurrentContent = null;
    }
}