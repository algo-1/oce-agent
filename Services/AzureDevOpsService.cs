using System.Net.Http.Headers;
using System.Text.Json;

namespace OncallAgent.Services;

public class AzureDevOpsService
{
    private readonly HttpClient _httpClient;
    private readonly string _organization;
    private readonly string _project;
    private readonly string _personalAccessToken;

    public AzureDevOpsService(HttpClient httpClient, string organization, string project, string personalAccessToken)
    {
        _httpClient = httpClient;
        _organization = organization;
        _project = project;
        _personalAccessToken = personalAccessToken;

        var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{_personalAccessToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
    }

    public async Task<string> GetWorkItemAsync(int workItemId)
    {
        var requestUrl = $"https://dev.azure.com/{_organization}/{_project}/_apis/wit/workitems/{workItemId}?api-version=6.0";
        var response = await _httpClient.GetAsync(requestUrl);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    public async Task<string> CreateWorkItemAsync(string title, string description)
    {
        var requestUrl = $"https://dev.azure.com/{_organization}/{_project}/_apis/wit/workitems/$Task?api-version=6.0";
        var workItem = new[]
        {
            new { op = "add", path = "/fields/System.Title", value = title },
            new { op = "add", path = "/fields/System.Description", value = description }
        };

        var content = new StringContent(JsonSerializer.Serialize(workItem), System.Text.Encoding.UTF8, "application/json-patch+json");
        var response = await _httpClient.PostAsync(requestUrl, content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return responseContent;
    }

    public async Task<string> CreateBugAsync(string title, string description)
    {
        var requestUrl = $"https://dev.azure.com/{_organization}/{_project}/_apis/wit/workitems/$Bug?api-version=6.0";
        var bug = new[]
        {
            new { op = "add", path = "/fields/System.Title", value = title },
            new { op = "add", path = "/fields/System.Description", value = description }
        };

        var content = new StringContent(JsonSerializer.Serialize(bug), System.Text.Encoding.UTF8, "application/json-patch+json");
        var response = await _httpClient.PostAsync(requestUrl, content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return responseContent;
    }
}
