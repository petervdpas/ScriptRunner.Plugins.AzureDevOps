using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ScriptRunner.Plugins.AzureDevOps.Models;

namespace ScriptRunner.Plugins.AzureDevOps.Interfaces;

/// <summary>
/// Defines methods for interacting with Azure DevOps queries and work items.
/// </summary>
public interface IDevOpsQueryService
{
    /// <summary>
    /// Replaces placeholders in a query template with actual values.
    /// </summary>
    /// <param name="queryTemplate">
    /// The query template containing placeholders, such as <c>@AREAPATH@</c>.
    /// </param>
    /// <returns>
    /// The query template with placeholders replaced, or <c>null</c> if the template is <c>null</c>.
    /// </returns>
    string? ReplaceAreaPath(string? queryTemplate);

    /// <summary>
    /// Executes a Work Item Query Language (WIQL) query in Azure DevOps.
    /// </summary>
    /// <param name="query">The WIQL query string to execute.</param>
    /// <returns>
    /// A <see cref="JArray"/> containing the query results, or <c>null</c> if the query fails.
    /// </returns>
    Task<JArray?> ExecuteQuery(string query);

    /// <summary>
    /// Fetches detailed information for a specific Azure DevOps work item.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to fetch.</param>
    /// <returns>
    /// A <see cref="WorkItemViewModel"/> containing detailed information about the work item,
    /// or <c>null</c> if the work item cannot be found.
    /// </returns>
    Task<WorkItemViewModel?> FetchWorkItemDetails(string workItemId);

    Task AddSavedQuery(SavedQuery query);

    Task<List<SavedQuery>> GetSavedQueries();

    Task UpdateSavedQuery(SavedQuery query);

    Task DeleteSavedQuery(Guid queryId);
}