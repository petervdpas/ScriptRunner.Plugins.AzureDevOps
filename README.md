# ScriptRunner.Plugins.AzureDevOps

![License](https://img.shields.io/badge/license-MIT-green)  
![Version](https://img.shields.io/badge/version-1.0.0-blue)

A powerful plugin for **ScriptRunner**, enabling seamless interaction with **Azure DevOps**.
This plugin simplifies tasks like managing queries, fetching work items, 
and editing configurations through a streamlined UI and database integration.

---

## 🚀 Features

- **Azure DevOps Integration**: Fetch, update, and manage work items and queries directly from Azure DevOps.
- **Query Management**: Save, load, and execute queries with customizable templates.
- **UI Integration**: Intuitive modal dialogs using Avalonia for managing Azure DevOps queries and work items.
- **Database Integration**: Persistent query storage using SQLite for efficient data handling.
- **Custom Query Support**: Replace placeholders dynamically and execute WIQL queries in Azure DevOps.
- **Extensibility**: Fully integrated with the **ScriptRunner** ecosystem, allowing for additional customizations.

---

## 📦 Installation

### Plugin Activation
Place the `ScriptRunner.Plugins.AzureDevOps` plugin assembly in the `Plugins` folder of your **ScriptRunner** project.
The plugin will be automatically discovered and activated.

---

## 📖 Usage

### Writing a Script

Here’s an example script to demonstrate using the **AzureDevOps Plugin**:

```csharp
/*
{
    "TaskCategory": "Plugins",
    "TaskName": "Azure DevOps Demo",
    "TaskDetail": "This script demonstrates the usage of Azure DevOps Plugin."
}
*/

var dialogService = ServiceLocator.GetService<IAzureDevOpsDialogService>();
var result = await dialogService.GetAzureDevOpsAsync(
    "Azure DevOps Interaction",
    width: 1280,
    height: 720
);

Console.WriteLine($"Dialog Result: {result}");
```

### Managing Queries

Queries can be managed via the plugin’s intuitive UI:
1. **Execute Query**: Replace placeholders and execute WIQL queries.
2. **Save Query**: Persist queries to the local SQLite database.
3. **Delete Query**: Remove queries from the database.

---

## 🔧 Configuration

### Plugin Settings

Configure the plugin through **ScriptRunner** settings. Here's an example configuration:

```json
[
  {
    "Key": "PluginName",
    "Type": "string",
    "Value": "AzureDevOps"
  },
  {
    "Key": "Organization",
    "Type": "string",
    "Value": "your-organization"
  },
  {
    "Key": "Project",
    "Type": "string",
    "Value": "your-project"
  },
  {
    "Key": "PersonalAccessToken",
    "Type": "string",
    "Value": "your-personal-access-token"
  },
  {
    "Key": "AreaPath",
    "Type": "string",
    "Value": "your-project\\TeamName\\AreaPath"
  },
  {
    "Key": "ApiEndpoint",
    "Type": "string",
    "Value": "https://dev.azure.com"
  },
  {
    "Key": "DbPath",
    "Type": "string",
    "Value": "azuredevops.db"
  },
  {
    "Key": "Timeout",
    "Type": "int",
    "Value": "30"
  }
]
```

### Database Integration

The plugin stores saved queries in an SQLite database specified by the `DbPath` configuration setting.
Queries can be saved, retrieved, and updated dynamically.

---

## 🌟 Advanced Features

### Custom Dialog Integration

Launch the Azure DevOps dialog to manage queries and work items:
```csharp
var dialogService = new AzureDevOpsDialogService(new DevOpsQueryService(configService, sqliteDatabase));
await dialogService.GetAzureDevOpsAsync("Azure DevOps Manager", 1280, 720);
```

### Query Execution

Replace placeholders and execute queries using the `DevOpsQueryService`:
```csharp
var queryService = new DevOpsQueryService(configService, sqliteDatabase);
var query = queryService.ReplaceAreaPath("SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.AreaPath] = '@AREAPATH@'");
var result = await queryService.ExecuteQuery(query);
```

### WIQL and Work Item Details

Fetch detailed work item data:
```csharp
var workItemDetails = await queryService.FetchWorkItemDetails("12345");
Console.WriteLine($"Title: {workItemDetails?.Title}, Description: {workItemDetails?.Details.Description}");
```

---

## 🔧 UI Dialog

The plugin provides a dialog for managing Azure DevOps queries and work items. The Avalonia-based UI allows for:
- Creating and saving queries.
- Executing WIQL queries.
- Viewing and editing work items.

---

## 📄 Contributing

1. Fork this repository.
2. Create a feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m 'Add YourFeature'`).
4. Push the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

---

## Author

Developed with **🧡 passion** by **Peter van de Pas**.
For any questions or feedback, feel free to open an issue or contact me directly!

---

## 🔗 Links

- [ScriptRunner Plugins Repository](https://github.com/petervdpas/ScriptRunner.Plugins)

---

## License

This project is licensed under the [MIT License](./LICENSE).
