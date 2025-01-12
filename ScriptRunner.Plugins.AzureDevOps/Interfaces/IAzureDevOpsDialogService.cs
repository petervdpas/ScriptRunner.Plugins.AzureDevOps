using System.Threading.Tasks;

namespace ScriptRunner.Plugins.AzureDevOps.Interfaces;

/// <summary>
/// Defines methods for displaying and interacting with an Azure DevOps dialog.
/// </summary>
public interface IAzureDevOpsDialogService
{
    /// <summary>
    /// Displays the Azure DevOps dialog and retrieves the result.
    /// </summary>
    /// <param name="title">The title of the dialog window. Defaults to "Azure DevOps".</param>
    /// <param name="width">The width of the dialog window in pixels. Defaults to 1280.</param>
    /// <param name="height">The height of the dialog window in pixels. Defaults to 720.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation. 
    /// The task result contains the string returned from the dialog, or <c>null</c> if the dialog is closed without a result.
    /// </returns>
    Task<string?> GetAzureDevOpsAsync(
        string title = "Azure DevOps",
        int width = 1280,
        int height = 720);
}