using System;
using System.Collections.Generic;
using System.Data;
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
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.AzureDevOps;

/// <summary>
/// Implements methods for interacting with Azure DevOps queries and work items.
/// </summary>
public class DevOpsQueryService : IDevOpsQueryService
{
    private readonly DevOpsConfigItem _config;
    private readonly ISqliteDatabase _database;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DevOpsQueryService"/> class.
    /// </summary>
    /// <param name="configService">The configuration service for retrieving Azure DevOps settings.</param>
    /// <param name="database">The SQLite database for persisting saved queries.</param>
    public DevOpsQueryService(IDevOpsConfigService configService, ISqliteDatabase database)
    {
        _config = configService.GetConfiguration();
        _database = database ?? throw new ArgumentNullException(nameof(database));

        if (string.IsNullOrEmpty(_config.DbPath))
        {
            throw new InvalidOperationException("Database path (DbPath) is not configured.");
        }

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
        var url = $"{_config.ApiEndpoint}/{_config.Organization}/{_config.Project}/_apis/wit/wiql?api-version=6.0";
        var client = new HttpClient();

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config.PersonalAccessToken}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var wiql = new
        {
            query
        };

        var jsonContent = new StringContent(JObject.FromObject(wiql).ToString(), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, jsonContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            try
            {
                var workItems = JObject.Parse(responseBody);
                return workItems["workItems"] as JArray;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return null;
            }
        }
        else
        {
            Console.WriteLine($"HTTP error: {response.StatusCode}");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<WorkItemViewModel?> FetchWorkItemDetails(string workItemId)
    {
        var url = $"{_config.ApiEndpoint}/{_config.Organization}/{_config.Project}/_apis/wit/workitems/{workItemId}?api-version=6.0";
        var client = new HttpClient();

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config.PersonalAccessToken}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var response = await client.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            try
            {
                var workItem = JObject.Parse(responseBody);
                var title = workItem["fields"]?["System.Title"]?.ToString();
                var fields = workItem["fields"]?.ToObject<Dictionary<string, object>>();

                var htmlDescription = workItem["fields"]?["System.Description"]?.ToString();

                // Convert HTML to plain text
                var description = htmlDescription != null ? HtmlToPlainText(htmlDescription) : null;

                var workItemViewModel = new WorkItemViewModel
                {
                    Id = workItemId,
                    Title = title,
                    Fields = fields,
                    Details = new DetailModel
                    {
                        Description = description
                    }
                };

                return workItemViewModel;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return null;
            }
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Authorization error: Invalid PAT or insufficient permissions.");
            return null;
        }

        Console.WriteLine($"HTTP error: {response.StatusCode}");
        return null;
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
}
