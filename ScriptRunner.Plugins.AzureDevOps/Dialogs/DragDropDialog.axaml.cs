using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ScriptRunner.Plugins.AzureDevOps.Dialogs;

/// <summary>
/// Represents a dialog window that supports drag-and-drop functionality.
/// </summary>
/// <remarks>
/// This window serves as the user interface for demonstrating drag-and-drop operations
/// using Avalonia behaviors. The XAML layout for this dialog is loaded during initialization.
/// </remarks>
public partial class DragDropDialog : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragDropDialog"/> class.
    /// </summary>
    /// <remarks>
    /// The constructor initializes the dialog and loads its associated XAML layout.
    /// </remarks>
    public DragDropDialog()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Loads the XAML layout for the <see cref="DragDropDialog"/>.
    /// </summary>
    /// <remarks>
    /// This method uses the <see cref="AvaloniaXamlLoader"/> to load the XAML content for this dialog.
    /// Ensure that the associated XAML file is correctly linked and contains valid markup.
    /// </remarks>
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}