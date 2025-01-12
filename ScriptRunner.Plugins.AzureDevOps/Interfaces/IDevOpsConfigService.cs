using ScriptRunner.Plugins.AzureDevOps.Models;

namespace ScriptRunner.Plugins.AzureDevOps.Interfaces;

/// <summary>
/// Interface for managing Azure DevOps configuration settings.
/// </summary>
public interface IDevOpsConfigService
{
    /// <summary>
    /// Retrieves the configuration settings required for interacting with Azure DevOps.
    /// </summary>
    /// <returns>A <see cref="DevOpsConfigItem"/> containing the configuration settings.</returns>
    DevOpsConfigItem GetConfiguration();
}