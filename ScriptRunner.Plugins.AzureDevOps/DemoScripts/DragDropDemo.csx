/*
{
    "TaskCategory": "Plugins",
    "TaskName": "Drag Drop Demo Script",
    "TaskDetail": "Test script for Drag and Drop.",
    "RequiredPlugins": ["AzureDevOps"]
}
*/

var logger = GetLogger("DragDropDemo");
var dragDropService = new DragDropService(logger: logger);
var ddDemo = new DragDropDemo(dragDropService: dragDropService, logger: logger);

var result = await ddDemo.DisplayDragDropDemoAsync(
    "Drag Drop Demo",
    width: 800,
    height: 600
);

return "Script run completed";