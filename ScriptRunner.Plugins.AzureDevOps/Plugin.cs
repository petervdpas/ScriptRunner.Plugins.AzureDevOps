using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Attributes;
using ScriptRunner.Plugins.AzureDevOps.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Tools;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
///     A plugin that registers and provides ...
/// </summary>
/// <remarks>
///     This plugin demonstrates how to ...
/// </remarks>
[PluginMetadata(
    "AzureDevOps",
    "A plugin to query Azure DevOps with script runner",
    "Peter van de Pas",
    "1.0.0",
    PluginSystemConstants.CurrentPluginSystemVersion,
    PluginSystemConstants.CurrentFrameworkVersion,
    services: ["IDevOpsQueryService", "IAzureDevOpsDialogService"])]
public class Plugin : BaseAsyncServicePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    public override string Name => "AzureDevOps";

    /// <summary>
    /// Asynchronously initializes the plugin using the provided configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    public override async Task InitializeAsync(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Store settings into LocalStorage
        PluginSettingsHelper.StoreSettings(configuration);
        
        // Optionally display the settings
        PluginSettingsHelper.DisplayStoredSettings();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously registers the plugin's services into the application's dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    public override async Task RegisterServicesAsync(IServiceCollection services)
    {
        // Simulate async service registration (e.g., initializing an external resource)
        await Task.Delay(50);
        
        // Register QueryService
        services.AddSingleton<IDevOpsQueryService, DevOpsQueryService>();
        
        services.AddSingleton<IAzureDevOpsDialogService>(sp =>
            new AzureDevOpsDialogService(sp.GetRequiredService<IDevOpsQueryService>()));
    }
    
    /// <summary>
    /// Asynchronously executes the plugin's main functionality.
    /// </summary>
    public override async Task ExecuteAsync()
    {
        // Example execution logic
        await Task.Delay(50);
        
        var storedSetting = PluginSettingsHelper.RetrieveSetting<string>("PluginName", true);
        Console.WriteLine($"Retrieved PluginName: {storedSetting}");
    }
}