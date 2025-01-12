using System;

namespace ScriptRunner.Plugins.AzureDevOps.Models;

/// <summary>
/// Represents a saved query for Azure DevOps.
/// </summary>
public class SavedQuery
{
    /// <summary>
    /// Gets the unique identifier for the saved query.
    /// </summary>
    /// <remarks>
    /// This identifier is generated automatically when a new instance is created.
    /// </remarks>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the saved query.
    /// </summary>
    /// <remarks>
    /// The name should provide a clear and descriptive label for the query's purpose.
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the query text for the saved query.
    /// </summary>
    /// <remarks>
    /// This text typically contains a Work Item Query Language (WIQL) statement or other API-specific query details.
    /// </remarks>
    public string? QueryText { get; set; }
}
