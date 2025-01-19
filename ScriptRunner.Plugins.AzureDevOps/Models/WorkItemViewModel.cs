using System.Collections.Generic;

namespace ScriptRunner.Plugins.AzureDevOps.Models;

/// <summary>
///     Represents the view model for a work item in Azure DevOps.
/// </summary>
public class WorkItemViewModel
{
    /// <summary>
    ///     Gets the unique identifier of the work item.
    /// </summary>
    /// <remarks>
    ///     This is typically the work item's numeric ID in Azure DevOps.
    /// </remarks>
    public string? Id { get; init; }

    /// <summary>
    ///     Gets the title of the work item.
    /// </summary>
    /// <remarks>
    ///     The title provides a brief summary of the work item, such as a task, bug, or user story.
    /// </remarks>
    public string? Title { get; init; }

    /// <summary>
    ///     Gets or sets the additional fields associated with the work item.
    /// </summary>
    /// <remarks>
    ///     This dictionary can contain key-value pairs for any custom or system-defined fields in Azure DevOps,
    ///     such as <c>System.AssignedTo</c> or <c>System.State</c>.
    /// </remarks>
    public Dictionary<string, object>? Fields { get; set; }

    /// <summary>
    ///     Gets the detailed information about the work item.
    /// </summary>
    /// <remarks>
    ///     The <see cref="Details" /> property includes extended information, such as the description.
    /// </remarks>
    public DetailModel? Details { get; init; } = new();

    /// <summary>
    ///     Gets a formatted string representing the work item as a list item.
    /// </summary>
    /// <remarks>
    ///     The format is <c>#{Id}: {Title}</c>, which can be used for display in a list or UI.
    /// </remarks>
    public string ListItem => $"#{Id}: {Title}";
}