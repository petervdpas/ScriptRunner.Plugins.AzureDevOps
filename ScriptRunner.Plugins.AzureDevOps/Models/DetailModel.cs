namespace ScriptRunner.Plugins.AzureDevOps.Models;

/// <summary>
///     Represents the detailed information about a work item in Azure DevOps.
/// </summary>
public class DetailModel
{
    /// <summary>
    ///     Gets or sets the description of the work item.
    /// </summary>
    /// <remarks>
    ///     The description provides additional context or instructions related to the work item.
    /// </remarks>
    public string? Description { get; set; }
}