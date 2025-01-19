using Avalonia.Controls;
using ScriptRunner.Plugins.AzureDevOps.ViewModels;

namespace ScriptRunner.Plugins.AzureDevOps.Dialogs;

/// <summary>
///     Represents the Azure DevOps dialog for managing and executing queries and viewing work items.
/// </summary>
/// <remarks>
///     This dialog provides a user interface for interacting with Azure DevOps, including:
///     - Executing and saving queries.
///     - Displaying and selecting work items from Azure DevOps queries.
///     The dialog uses a tabbed layout with two main sections:
///     - Queries: Allows the user to create, execute, and save queries.
///     - WorkItems: Displays a list of work items with detailed information about the selected item.
///     Data Context:
///     - The dialog is bound to an instance of <see cref="AzureDevOpsModel" />, which serves as its ViewModel.
///     - ViewModel commands and properties control query execution and work item display.
/// </remarks>
public partial class AzureDevOpsDialog : Window
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AzureDevOpsDialog" /> class.
    /// </summary>
    /// <remarks>
    ///     The constructor initializes the dialog and its components.
    ///     The XAML layout is loaded via <see cref="InitializeComponent" />.
    /// </remarks>
    public AzureDevOpsDialog()
    {
        InitializeComponent();
    }
}