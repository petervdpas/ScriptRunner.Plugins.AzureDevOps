using System;
using System.Collections.Generic;
using ScriptRunner.Plugins.AzureDevOps.Interfaces;
using ScriptRunner.Plugins.AzureDevOps.Models;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
/// Service for managing Azure DevOps configuration settings.
/// </summary>
public class DevOpsConfigService : IDevOpsConfigService
{
    private readonly ILocalStorage _localStorage;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevOpsConfigService"/> class.
    /// </summary>
    /// <param name="localStorage">The local storage instance for retrieving configuration values.</param>
    public DevOpsConfigService(ILocalStorage localStorage)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    /// <inheritdoc />
    public DevOpsConfigItem GetConfiguration()
    {
        return new DevOpsConfigItem
        {
            Organization = PluginSettingsHelper.RetrieveSetting<string>(_localStorage, "Organization"),
            Project = PluginSettingsHelper.RetrieveSetting<string>(_localStorage, "Project"),
            PersonalAccessToken = PluginSettingsHelper.RetrieveSetting<string>(_localStorage, "PersonalAccessToken"),
            AreaPath = PluginSettingsHelper.RetrieveSetting<string>(_localStorage, "AreaPath"),
            ApiEndpoint = PluginSettingsHelper.RetrieveSetting<string>(_localStorage, "ApiEndpoint"),
            Timeout = PluginSettingsHelper.RetrieveSetting<int>(_localStorage, "Timeout"),
            DbPath = PluginSettingsHelper.RetrieveSetting<string>(_localStorage, "DbPath")
        };
    }
}