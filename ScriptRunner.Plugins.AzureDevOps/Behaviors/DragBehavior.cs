using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.AzureDevOps.Behaviors;

/// <summary>
/// A behavior that enables drag-and-drop functionality for a control in Avalonia.
/// </summary>
public class DragBehavior : Behavior<Control>
{
    private IPluginLogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DragBehavior"/> class.
    /// </summary>
    /// <param name="logger">
    /// An optional logger instance implementing <see cref="IPluginLogger"/> used to log drag-and-drop events.
    /// If no logger is provided, logging will be disabled.
    /// </param>
    public DragBehavior(IPluginLogger? logger = null)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DragBehavior"/> class.
    /// </summary>
    public DragBehavior()
    {
    }
    
    /// <summary>
    /// Sets the logger for drag-and-drop events.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public void SetLogger(IPluginLogger? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Defines the <see cref="DragData"/> property that stores the data to be dragged.
    /// </summary>
    public static readonly StyledProperty<object?> DragDataProperty =
        AvaloniaProperty.Register<DragBehavior, object?>(nameof(DragData));

    /// <summary>
    /// Gets or sets the data to be used for the drag operation.
    /// </summary>
    public object? DragData
    {
        get => GetValue(DragDataProperty);
        set => SetValue(DragDataProperty, value);
    }

    /// <summary>
    /// Called when the behavior is attached to a control.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null) return;
        
        _logger?.Information($"DragBehavior attached to {AssociatedObject}");
        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Called when the behavior is detached from a control.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject == null) return;
        
        _logger?.Information($"DragBehavior detached from {AssociatedObject}");
        AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
    }

    /// <summary>
    /// Handles the PointerPressed event and initiates a drag-and-drop operation.
    /// </summary>
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _logger?.Information($"PointerPressed on {AssociatedObject}");
        if (DragData == null)
        {
            _logger?.Warning("DragData is null; drag operation aborted.");
            return;
        }

        _logger?.Information($"Drag operation started with data: {DragData}");
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Text, DragData);

        DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
    }
}