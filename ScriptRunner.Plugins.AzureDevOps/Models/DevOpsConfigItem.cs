namespace ScriptRunner.Plugins.AzureDevOps.Models;

/// <summary>
///     Represents the configuration settings required for interacting with Azure DevOps.
/// </summary>
public class DevOpsConfigItem
{
    /// <summary>
    ///     Gets or sets the Azure DevOps organization name.
    /// </summary>
    /// <remarks>
    ///     This typically corresponds to the organization URL, such as <c>https://dev.azure.com/{organization}</c>.
    /// </remarks>
    public string? Organization { get; set; }

    /// <summary>
    ///     Gets or sets the Azure DevOps project name.
    /// </summary>
    /// <remarks>
    ///     This is the project within the organization where work items, builds, and other resources are located.
    /// </remarks>
    public string? Project { get; set; }

    /// <summary>
    ///     Gets or sets the personal access token (PAT) for authenticating with Azure DevOps.
    /// </summary>
    /// <remarks>
    ///     Ensure the token has sufficient permissions for the desired API operations, such as work item read/write.
    /// </remarks>
    public string? PersonalAccessToken { get; set; }

    /// <summary>
    ///     Gets or sets the area path within the project to scope work items or queries.
    /// </summary>
    /// <remarks>
    ///     Example: <c>ProjectName\TeamName\AreaPath</c>.
    /// </remarks>
    public string? AreaPath { get; set; }

    /// <summary>
    ///     Gets or sets the base API endpoint for Azure DevOps.
    /// </summary>
    /// <remarks>
    ///     Defaults to the Azure DevOps REST API base URL, such as <c>https://dev.azure.com</c>. This can be overridden if
    ///     needed.
    /// </remarks>
    public string? ApiEndpoint { get; set; }

    /// <summary>
    ///     Gets or sets the timeout value (in seconds) for API requests.
    /// </summary>
    /// <remarks>
    ///     The default value is <c>30</c> seconds.
    /// </remarks>
    public int Timeout { get; set; } = 30;

    /// <summary>
    ///     Gets or sets the database file path for storing plugin data such as saved queries.
    /// </summary>
    public string? DbPath { get; set; }
}