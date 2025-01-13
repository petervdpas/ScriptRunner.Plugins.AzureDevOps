using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScriptRunner.Plugins.AzureDevOps.Interfaces;
using ScriptRunner.Plugins.AzureDevOps.Models;
using ScriptRunner.Plugins.Tools;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
/// Implements methods for interacting with Azure DevOps queries and work items.
/// </summary>
public class DevOpsQueryService : IDevOpsQueryService
{
    private readonly DevOpsConfigItem _config;
    private readonly SqliteDatabase _database;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevOpsQueryService"/> class.
    /// </summary>
    public DevOpsQueryService()
    {
        _httpClient = new HttpClient();
        _config = DevOpsConfigHelper.GetConfiguration();

        ValidateConfiguration();
        ValidateDatabasePath();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config.PersonalAccessToken}"))
        );

        _database = new SqliteDatabase();
        _database.Setup($"Data Source={_config.DbPath}");
        InitializeDatabase();
    }

    /// <inheritdoc />
    public string? ReplaceAreaPath(string? queryTemplate)
    {
        return queryTemplate?.Replace("@AREAPATH@", _config.AreaPath);
    }
    
    /// <inheritdoc />
    public async Task<JArray?> ExecuteQuery(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));
        }

        var url = $"{_config.ApiEndpoint}/{_config.Organization}/{_config.Project}/_apis/wit/wiql?api-version=6.0";
        var wiql = new { query };
        var jsonContent = new StringContent(JObject.FromObject(wiql).ToString(), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, jsonContent);
        ValidateHttpResponse(response, url);

        try
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var workItems = JObject.Parse(responseBody);
            return workItems["workItems"] as JArray;
        }
        catch (JsonReaderException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<WorkItemViewModel?> FetchWorkItemDetails(string workItemId)
    {
        var url = $"{_config.ApiEndpoint}/{_config.Organization}/{_config.Project}/_apis/wit/workitems/{workItemId}?api-version=6.0";
        var response = await _httpClient.GetAsync(url);
        ValidateHttpResponse(response, url);

        try
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var workItem = JObject.Parse(responseBody);
            var title = workItem["fields"]?["System.Title"]?.ToString();
            var fields = workItem["fields"]?.ToObject<Dictionary<string, object>>();
            var htmlDescription = workItem["fields"]?["System.Description"]?.ToString();
            var description = htmlDescription != null ? HtmlToPlainText(htmlDescription) : null;

            return new WorkItemViewModel
            {
                Id = workItemId,
                Title = title,
                Fields = fields,
                Details = new DetailModel
                {
                    Description = description
                }
            };
        }
        catch (JsonReaderException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Adds a new saved query to the SQLite database.
    /// </summary>
    public async Task AddSavedQuery(SavedQuery query)
    {
        _database.OpenConnection();

        const string insertQuery = """
                                   INSERT INTO SavedQueries (Id, Name, QueryText)
                                   VALUES (@Id, @Name, @QueryText)
                                   """;
        var parameters = new Dictionary<string, object?>
        {
            { "@Id", query.Id.ToString() },
            { "@Name", query.Name },
            { "@QueryText", query.QueryText }
        };

        _database.ExecuteNonQuery(insertQuery, parameters!);
        _database.CloseConnection();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves all saved queries from the SQLite database.
    /// </summary>
    public async Task<List<SavedQuery>> GetSavedQueries()
    {
        _database.OpenConnection();

        const string selectQuery = "SELECT Id, Name, QueryText FROM SavedQueries";
        var dataTable = _database.ExecuteQuery(selectQuery);

        var savedQueries = (from DataRow row in dataTable.Rows
            select new SavedQuery
            {
                Id = Guid.Parse(row["Id"].ToString() ?? string.Empty),
                Name = row["Name"].ToString(),
                QueryText = row["QueryText"].ToString()
            }).ToList();

        _database.CloseConnection();
        return await Task.FromResult(savedQueries);
    }

    /// <summary>
    /// Updates an existing saved query in the SQLite database.
    /// </summary>
    public async Task UpdateSavedQuery(SavedQuery query)
    {
        _database.OpenConnection();

        const string updateQuery = """
                                   UPDATE SavedQueries
                                   SET Name = @Name, 
                                       QueryText = @QueryText
                                   WHERE Id = @Id
                                   """;
        var parameters = new Dictionary<string, object?>
        {
            { "@Id", query.Id.ToString() },
            { "@Name", query.Name },
            { "@QueryText", query.QueryText }
        };

        _database.ExecuteNonQuery(updateQuery, parameters!);
        _database.CloseConnection();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a saved query from the SQLite database by its ID.
    /// </summary>
    public async Task DeleteSavedQuery(Guid queryId)
    {
        _database.OpenConnection();

        const string deleteQuery = "DELETE FROM SavedQueries WHERE Id = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "@Id", queryId.ToString() }
        };

        _database.ExecuteNonQuery(deleteQuery, parameters);
        _database.CloseConnection();

        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Converts an HTML string to plain text.
    /// </summary>
    /// <param name="html">The HTML string to convert.</param>
    /// <returns>The plain text representation of the HTML.</returns>
    private static string HtmlToPlainText(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var sb = new StringBuilder();
        ConvertToPlainText(doc.DocumentNode, sb);
        return sb.ToString();
    }

    /// <summary>
    /// Recursively converts HTML nodes to plain text.
    /// </summary>
    /// <param name="node">The current HTML node.</param>
    /// <param name="sb">The <see cref="StringBuilder"/> to append the plain text to.</param>
    private static void ConvertToPlainText(HtmlNode? node, StringBuilder sb)
    {
        if (node == null)
            return;

        switch (node.NodeType)
        {
            case HtmlNodeType.Document:
                foreach (var child in node.ChildNodes)
                    ConvertToPlainText(child, sb);
                break;

            case HtmlNodeType.Element:
                var tagName = node.Name.ToLower();
                switch (tagName)
                {
                    case "br":
                        sb.AppendLine();
                        break;
                    case "p":
                    case "div":
                        if (sb.Length > 0)
                            sb.AppendLine();
                        foreach (var child in node.ChildNodes)
                            ConvertToPlainText(child, sb);
                        break;
                    case "li":
                        sb.Append("- ");
                        foreach (var child in node.ChildNodes)
                            ConvertToPlainText(child, sb);
                        break;
                    case "ul":
                    case "ol":
                        foreach (var child in node.ChildNodes)
                            ConvertToPlainText(child, sb);
                        break;
                    default:
                        foreach (var child in node.ChildNodes)
                            ConvertToPlainText(child, sb);
                        break;
                }
                break;

            case HtmlNodeType.Text:
                var text = ((HtmlTextNode)node).Text;
                if (!string.IsNullOrWhiteSpace(text))
                    sb.Append(HtmlEntity.DeEntitize(text));
                break;
        }
    }
    
    /// <summary>
    /// Ensures the database schema for saved queries is created.
    /// </summary>
    private void InitializeDatabase()
    {
        _database.OpenConnection();
        const string createTableQuery = """
                                        CREATE TABLE IF NOT EXISTS SavedQueries (
                                            Id TEXT PRIMARY KEY,
                                            Name TEXT NOT NULL,
                                            QueryText TEXT NOT NULL
                                        )
                                        """;
        _database.ExecuteNonQuery(createTableQuery);
        _database.CloseConnection();
    }
    
    /// <summary>
    /// Validates the essential configuration properties.
    /// </summary>
    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_config.ApiEndpoint))
            throw new InvalidOperationException("API Endpoint is not configured.");
        if (string.IsNullOrWhiteSpace(_config.Organization))
            throw new InvalidOperationException("Organization is not configured.");
        if (string.IsNullOrWhiteSpace(_config.Project))
            throw new InvalidOperationException("Project is not configured.");
        if (string.IsNullOrWhiteSpace(_config.PersonalAccessToken))
            throw new InvalidOperationException("Personal Access Token (PAT) is not configured.");
    }

    /// <summary>
    /// Validates the HTTP response and logs an appropriate message.
    /// </summary>
    private static void ValidateHttpResponse(HttpResponseMessage response, string url)
    {
        if (response.IsSuccessStatusCode) return;
        
        var error = $"HTTP request failed with status code {response.StatusCode} for URL: {url}";
        Console.WriteLine(error);
        throw new HttpRequestException(error);
    }

    /// <summary>
    /// Validates the database path.
    /// </summary>
    private void ValidateDatabasePath()
    {
        if (string.IsNullOrWhiteSpace(_config.DbPath))
            throw new InvalidOperationException("Database path (DbPath) is not configured.");

        // If the path is not absolute, treat it as relative to the application's base directory
        if (!Path.IsPathRooted(_config.DbPath))
        {
            var defaultScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScriptRunnerScripts");
            _config.DbPath = Path.Combine(defaultScriptPath, _config.DbPath);
        }

        // Ensure the directory exists or create it
        var directory = Path.GetDirectoryName(_config.DbPath);
        if (directory == null)
            throw new InvalidOperationException($"Invalid database path: {_config.DbPath}");

        if (Directory.Exists(directory)) return;
        
        Console.WriteLine($"Directory does not exist. Creating: {directory}");
        Directory.CreateDirectory(directory);
    }
}
