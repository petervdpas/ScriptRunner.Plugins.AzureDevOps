using ReactiveUI;

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
}