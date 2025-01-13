using ScriptRunner.Plugins.AzureDevOps.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
/// Service for managing Azure DevOps configuration settings.
/// </summary>
public static class DevOpsConfigHelper
{
    /// <summary>
    /// Retrieves the configuration settings required for interacting with Azure DevOps.
    /// </summary>
    /// <returns>A <see cref="DevOpsConfigItem"/> containing the configuration settings.</returns>
    public static DevOpsConfigItem GetConfiguration()
    {
        return new DevOpsConfigItem
        {
            Organization = PluginSettingsHelper.RetrieveSetting<string>("Organization"),
            Project = PluginSettingsHelper.RetrieveSetting<string>("Project"),
            PersonalAccessToken = PluginSettingsHelper.RetrieveSetting<string>("PersonalAccessToken"),
            AreaPath = PluginSettingsHelper.RetrieveSetting<string>("AreaPath"),
            ApiEndpoint = PluginSettingsHelper.RetrieveSetting<string>("ApiEndpoint"),
            Timeout = PluginSettingsHelper.RetrieveSetting<int>("Timeout"),
            DbPath = PluginSettingsHelper.RetrieveSetting<string>("DbPath")
        };
    }
}