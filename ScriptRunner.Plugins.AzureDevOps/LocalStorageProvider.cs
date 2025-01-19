using System;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
///     Provides a static mechanism for managing a shared <see cref="ILocalStorage" /> instance across the Azure DevOps
///     plugin.
/// </summary>
public static class LocalStorageProvider
{
    private static ILocalStorage? _localStorage;

    /// <summary>
    ///     Sets the LocalStorage instance.
    /// </summary>
    /// <param name="localStorage">The LocalStorage instance to set.</param>
    public static void Initialize(ILocalStorage localStorage)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    /// <summary>
    ///     Gets the LocalStorage instance.
    /// </summary>
    /// <returns>The LocalStorage instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the LocalStorage instance has not been initialized.
    /// </exception>
    public static ILocalStorage Get()
    {
        return _localStorage ?? throw new InvalidOperationException("LocalStorage has not been initialized.");
    }
}