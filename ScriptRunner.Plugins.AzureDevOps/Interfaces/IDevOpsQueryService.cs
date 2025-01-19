using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ScriptRunner.Plugins.AzureDevOps.Models;

namespace ScriptRunner.Plugins.AzureDevOps.Interfaces;

/// <summary>
///     Defines methods for interacting with Azure DevOps queries and work items.
/// </summary>
public interface IDevOpsQueryService
{
    /// <summary>
    ///     Replaces placeholders in a query template with actual values.
    /// </summary>
    /// <param name="queryTemplate">
    ///     The query template containing placeholders, such as <c>@AREAPATH@</c>.
    /// </param>
    /// <returns>
    ///     The query template with placeholders replaced, or <c>null</c> if the template is <c>null</c>.
    /// </returns>
    string? ReplaceAreaPath(string? queryTemplate);

    /// <summary>
    ///     Executes a Work Item Query Language (WIQL) query in Azure DevOps.
    /// </summary>
    /// <param name="query">The WIQL query string to execute.</param>
    /// <returns>
    ///     A <see cref="JArray" /> containing the query results, or <c>null</c> if the query fails.
    /// </returns>
    Task<JArray?> ExecuteQuery(string query);

    /// <summary>
    ///     Fetches detailed information for a specific Azure DevOps work item.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to fetch.</param>
    /// <returns>
    ///     A <see cref="WorkItemViewModel" /> containing detailed information about the work item,
    ///     or <c>null</c> if the work item cannot be found.
    /// </returns>
    Task<WorkItemViewModel?> FetchWorkItemDetails(string workItemId);

    /// <summary>
    ///     Adds a new saved query to the database.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="SavedQuery" /> object to be added. The query includes a unique identifier, name, and query text.
    /// </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    Task AddSavedQuery(SavedQuery query);

    /// <summary>
    ///     Retrieves all saved queries from the database.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task" /> that represents the asynchronous operation. The task result contains a list of
    ///     <see cref="SavedQuery" /> objects.
    /// </returns>
    Task<List<SavedQuery>> GetSavedQueries();

    /// <summary>
    ///     Updates an existing saved query in the database.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="SavedQuery" /> object containing updated information. The query must have a valid identifier.
    /// </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    Task UpdateSavedQuery(SavedQuery query);

    /// <summary>
    ///     Deletes a saved query from the database by its unique identifier.
    /// </summary>
    /// <param name="queryId">
    ///     The <see cref="Guid" /> representing the unique identifier of the saved query to delete.
    /// </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    Task DeleteSavedQuery(Guid queryId);
}