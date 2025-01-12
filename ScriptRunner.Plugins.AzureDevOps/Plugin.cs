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
    "A plugin that provides...",
    "Peter van de Pas",
    "1.0.0",
    PluginSystemConstants.CurrentPluginSystemVersion,
    PluginSystemConstants.CurrentFrameworkVersion,
    [])]
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
        if (LocalStorage == null)
        {
            throw new InvalidOperationException(
                "LocalStorage has not been initialized. " +
                "Ensure the host injects LocalStorage before calling InitializeAsync.");
        }
        
        // Store settings into LocalStorage
        PluginSettingsHelper.StoreSettings(LocalStorage, configuration);

        // Optionally display the settings
        PluginSettingsHelper.DisplayStoredSettings(LocalStorage);
        
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
        
        // Register the service
        services.AddSingleton<IDevOpsConfigService>(_ => new DevOpsConfigService(LocalStorage));
        
        // Register ISqliteDatabase with the plugin's DbPath
        var dbPath = PluginSettingsHelper.RetrieveSetting<string>(LocalStorage, "DbPath");
        if (string.IsNullOrEmpty(dbPath))
        {
            throw new InvalidOperationException("DbPath is not configured in LocalStorage.");
        }
        var sqliteDatabase = new SqliteDatabase();
        sqliteDatabase.Setup($"Data Source={dbPath}");

        // Register QueryService
        services.AddSingleton<IDevOpsQueryService>(provider =>
        {
            var configService = provider.GetRequiredService<IDevOpsConfigService>();
            return new DevOpsQueryService(configService, sqliteDatabase);
        });
    }
    
    /// <summary>
    /// Asynchronously executes the plugin's main functionality.
    /// </summary>
    public override async Task ExecuteAsync()
    {
        // Example execution logic
        await Task.Delay(50);
        
        var storedSetting = PluginSettingsHelper.RetrieveSetting<string>(LocalStorage, "PluginName");
        Console.WriteLine($"Retrieved PluginName: {storedSetting}");
    }
}