using System;
using System.Threading.Tasks;
using ScriptRunner.Plugins.AzureDevOps.Dialogs;
using ScriptRunner.Plugins.AzureDevOps.Interfaces;
using ScriptRunner.Plugins.AzureDevOps.ViewModels;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
///     Implements methods for displaying and interacting with an Azure DevOps dialog.
/// </summary>
public class AzureDevOpsDialogService : IAzureDevOpsDialogService
{
    private readonly IDevOpsQueryService _queryService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AzureDevOpsDialogService" /> class.
    /// </summary>
    /// <param name="queryService">The query service used for retrieving and managing Azure DevOps data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="queryService" /> is <c>null</c>.
    /// </exception>
    public AzureDevOpsDialogService(IDevOpsQueryService queryService)
    {
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
    }

    /// <inheritdoc />
    public async Task<string?> GetAzureDevOpsAsync(
        string title = "Azure DevOps",
        int width = 1280,
        int height = 720)
    {
        // Create and configure the Azure DevOps dialog
        var dialog = new AzureDevOpsDialog
        {
            Title = title,
            Width = width,
            Height = height
        };

        // Set the data context to a new AzureDevOpsModel instance
        var viewModel = new AzureDevOpsModel(dialog, _queryService);
        dialog.DataContext = viewModel;

        // Display the dialog and return the result
        return await DialogHelper.ShowDialogAsync(dialog.ShowDialog<string?>);
    }
}