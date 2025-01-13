/*
{
    "TaskCategory": "Plugins",
    "TaskName": "AzureDevOps Demo Script",
    "TaskDetail": "Test script for the AzureDevOps plugin.",
    "RequiredPlugins": ["AzureDevOps"]
}
*/

var queryService = new DevOpsQueryService();
var dialogService = new AzureDevOpsDialogService(queryService);

var result = await dialogService.GetAzureDevOpsAsync(
    "Azure DevOps Interaction",
    width: 1280,
    height: 720
);

return "Script run completed";