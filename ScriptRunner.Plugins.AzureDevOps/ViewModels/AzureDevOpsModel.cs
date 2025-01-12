using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ScriptRunner.Plugins.AzureDevOps.Interfaces;
using ScriptRunner.Plugins.AzureDevOps.Models;

namespace ScriptRunner.Plugins.AzureDevOps.ViewModels;

/// <summary>
/// Represents the ViewModel for Azure DevOps integration, handling query execution, management, and work item display.
/// </summary>
public class AzureDevOpsModel : ReactiveObject
{
    private readonly IDevOpsQueryService _devOpsQueryService;

    private WorkItemViewModel? _selectedItem;
    private int _selectedTabIndex;
    private SavedQuery? _selectedSavedQuery;
    private SavedQuery? _currentQuery;

    /// <summary>
    /// Gets the command for executing a query.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ExecuteQueryCommand { get; }

    /// <summary>
    /// Gets the command for saving a query.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveQueryCommand { get; }

    /// <summary>
    /// Gets the command for deleting a query.
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteQueryCommand { get; }

    /// <summary>
    /// Gets the command for loading all saved queries.
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadQueriesCommand { get; }

    /// <summary>
    /// Gets the collection of work items retrieved from Azure DevOps.
    /// </summary>
    public ObservableCollection<WorkItemViewModel> WorkItems { get; } = new();

    /// <summary>
    /// Gets the collection of saved queries.
    /// </summary>
    public ObservableCollection<SavedQuery?> SavedQueries { get; } = new();

    /// <summary>
    /// Gets or sets the currently selected work item.
    /// </summary>
    public WorkItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    /// <summary>
    /// Gets or sets the currently selected tab index in the UI.
    /// </summary>
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    /// <summary>
    /// Gets or sets the currently selected saved query.
    /// </summary>
    public SavedQuery? SelectedSavedQuery
    {
        get => _selectedSavedQuery;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedSavedQuery, value);
            if (value != null)
            {
                CurrentQuery = new SavedQuery
                {
                    Name = value.Name,
                    QueryText = value.QueryText
                };
            }
        }
    }

    /// <summary>
    /// Gets or sets the current query being edited or executed.
    /// </summary>
    public SavedQuery? CurrentQuery
    {
        get => _currentQuery;
        set => this.RaiseAndSetIfChanged(ref _currentQuery, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureDevOpsModel"/> class.
    /// </summary>
    /// <param name="devOpsQueryService">The service used to interact with Azure DevOps queries and work items.</param>
    public AzureDevOpsModel(IDevOpsQueryService devOpsQueryService)
    {
        _devOpsQueryService = devOpsQueryService;

        CurrentQuery = new SavedQuery
        {
            Name = "Committed WorkItems",
            QueryText =
                $"SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.State] = 'Committed' AND [System.AreaPath] = '@AREAPATH@' ORDER BY [System.Id]"
        };

        ExecuteQueryCommand = ReactiveCommand.CreateFromTask(ExecuteQuery);
        SaveQueryCommand = ReactiveCommand.CreateFromTask(SaveQuery);
        DeleteQueryCommand = ReactiveCommand.CreateFromTask(DeleteQuery);
        LoadQueriesCommand = ReactiveCommand.CreateFromTask(LoadQueries);

        LoadQueriesCommand.Execute().Subscribe();
    }

    /// <summary>
    /// Executes the current query and retrieves work items from Azure DevOps.
    /// </summary>
    private async Task ExecuteQuery()
    {
        WorkItems.Clear();

        if (CurrentQuery == null) return;

        var query = _devOpsQueryService.ReplaceAreaPath(CurrentQuery.QueryText);
        if (query != null)
        {
            var workItemsArray = await _devOpsQueryService.ExecuteQuery(query);

            if (workItemsArray != null)
            {
                foreach (var workItem in workItemsArray)
                {
                    var workItemId = workItem["id"]?.ToString();
                    if (workItemId == null) continue;

                    var workItemDetails = await _devOpsQueryService.FetchWorkItemDetails(workItemId);

                    WorkItems.Add(workItemDetails ?? new WorkItemViewModel
                    {
                        Id = workItemId,
                        Title = "Error fetching details"
                    });
                }
            }
            else
            {
                WorkItems.Add(new WorkItemViewModel { Id = "Error", Title = "Error executing query" });
            }
        }

        if (WorkItems.Count > 0)
        {
            SelectedItem = WorkItems[0];
            SelectedTabIndex = 1; // Switch to WorkItems Tab
        }
    }

    /// <summary>
    /// Saves the current query to the database.
    /// </summary>
    private async Task SaveQuery()
    {
        if (CurrentQuery == null || string.IsNullOrWhiteSpace(CurrentQuery.Name)) return;

        var existingQuery = SavedQueries.FirstOrDefault(q => q?.Name == CurrentQuery.Name);
        if (existingQuery != null)
        {
            Console.WriteLine("A query with this name already exists. Please change the name and try again.");
            return;
        }

        await _devOpsQueryService.AddSavedQuery(CurrentQuery);
        SavedQueries.Add(new SavedQuery
        {
            Id = CurrentQuery.Id,
            Name = CurrentQuery.Name,
            QueryText = CurrentQuery.QueryText
        });
    }

    /// <summary>
    /// Deletes the currently selected saved query.
    /// </summary>
    private async Task DeleteQuery()
    {
        if (SelectedSavedQuery == null) return;

        await _devOpsQueryService.DeleteSavedQuery(SelectedSavedQuery.Id);
        SavedQueries.Remove(SelectedSavedQuery);
    }

    /// <summary>
    /// Loads all saved queries from the database.
    /// </summary>
    private async Task LoadQueries()
    {
        SavedQueries.Clear();

        var queries = await _devOpsQueryService.GetSavedQueries();
        foreach (var query in queries)
        {
            SavedQueries.Add(query);
        }
    }
}