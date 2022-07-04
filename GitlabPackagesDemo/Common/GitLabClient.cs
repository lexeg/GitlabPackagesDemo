using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GitlabPackagesDemo.GitLab;
using GitlabPackagesDemo.Settings;
using Newtonsoft.Json;

namespace GitlabPackagesDemo.Common;

public class GitLabClient : IDisposable
{
    private readonly GitLabSettings _settings;
    private readonly string _hostAddress;
    private readonly HttpClient _httpClient;

    public GitLabClient(GitLabSettings settings)
    {
        _settings = settings;
        _hostAddress = CreateGitLabUri(_settings);
        _httpClient = CreateHttpClient(_settings.PrivateToken);
    }

    /*
     * curl --location --request GET 'https://gitlab-orms.ad.speechpro.com/api/v4/projects?private_token=<your_access_token>'
     */
    public async Task<GitRepository[]> GetProjects()
    {
        const int maxEntriesPerPage = 100;
        var projects = new List<GitRepository>();
        var count = await GetProjectCount();
        var pageCount = Math.Ceiling(count / (decimal)maxEntriesPerPage);
        for (var page = 1; page <= pageCount; page++)
        {
            var responseMessage = await _httpClient.GetAsync(
                $"{_hostAddress}?private_token={_settings.PrivateToken}&order_by=id&per_page={maxEntriesPerPage}&page={page}");
            if (!responseMessage.IsSuccessStatusCode) throw new Exception(nameof(GetProjects));
            var content = await responseMessage.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<GitRepository[]>(content) ?? Array.Empty<GitRepository>();
            projects.AddRange(items);
        }

        return projects.ToArray();
    }

    /*
     * curl --location --request GET 'https://gitlab-orms.ad.speechpro.com/api/v4/projects/157/search?scope=blobs&search=.csproj%20filename:*.csproj' \
--header 'PRIVATE-TOKEN: <your_access_token>'
     */
    public async Task<RepositoryFileData[]> SearchFilesInProject(int projectId, string searchText, string fileExtension)
    {
        var encodedSearch = Uri.EscapeDataString($"{searchText} filename:*.{fileExtension}");
        var responseMessage =
            await _httpClient.GetAsync($"{_hostAddress}/{projectId}/search?scope=blobs&search={encodedSearch}");
        if (!responseMessage.IsSuccessStatusCode) throw new Exception(nameof(SearchFilesInProject));
        var content = await responseMessage.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<RepositoryFileData[]>(content);
    }

    /*
     * curl --location --request GET 'https://gitlab-orms.ad.speechpro.com/api/v4/projects/157/repository/files/CloudSal.RecognitionManagementService.Migrations%2FCloudSal.RecognitionManagementService.Migrations.csproj/raw?ref=dev' \
--header 'PRIVATE-TOKEN: <your_access_token>'
     */
    public async Task<string> GetFileByName(int projectId, string fileName, string branch)
    {
        var encodedFileName = Uri.EscapeDataString(fileName);
        var responseMessage =
            await _httpClient.GetAsync(
                $"{_hostAddress}/{projectId}/repository/files/{encodedFileName}/raw?ref={branch}");
        if (!responseMessage.IsSuccessStatusCode) throw new Exception(nameof(GetFileByName));
        return await responseMessage.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    private string CreateGitLabUri(GitLabSettings settings)
    {
        var uriBuilder = new StringBuilder();
        uriBuilder.Append(settings.Host.TrimEnd('/'));
        uriBuilder.Append("/api/v4/projects");
        return uriBuilder.ToString();
    }
    
    private HttpClient CreateHttpClient(string privateToken)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateToken);
        return httpClient;
    }

    private async Task<int> GetProjectCount()
    {
        var responseMessage = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"{_hostAddress}"));
        if (!responseMessage.IsSuccessStatusCode) throw new Exception(nameof(GetProjectCount));
        var totalAsString = responseMessage.Headers.GetValues("X-Total").Single();
        return Convert.ToInt32(totalAsString);
    }
}