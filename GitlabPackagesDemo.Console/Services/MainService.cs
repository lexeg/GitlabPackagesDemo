using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.Common.Data;
using GitlabPackagesDemo.Common.Extensions;
using GitlabPackagesDemo.Common.GitLab;
using GitlabPackagesDemo.Common.Settings;
using Microsoft.Extensions.Options;

namespace GitlabPackagesDemo.Console.Services;

class MainService : IMainService
{
    private readonly IOptions<GitLabSettings> _gitLabSettings;
    private readonly IOptions<SearchSettings> _searchSettings;

    public MainService(IOptions<GitLabSettings> gitLabSettings, IOptions<SearchSettings> searchSettings)
    {
        _gitLabSettings = gitLabSettings;
        _searchSettings = searchSettings;
    }

    public async Task CreatePackagesFile(string filePath, bool writeFullPath)
    {
        //DI-containers
        var settings = _searchSettings.Value;
        using var client = new GitLabClient(_gitLabSettings.Value);
        GitRepository[] repositories = await client.GetProjects();
        RepositoryService repositoryService = new RepositoryService();
        RepoFiles[] filesInProject = await repositoryService.GetFilesInProject(client, settings, repositories);
        var filesContent = await repositoryService.GetFilesContent(client, filesInProject);
        PackageProjects[] packageProjects = filesContent.GetPackageProjects();

        FileSaver fileSaver = new FileSaver();
        await fileSaver.CreatePackagesFile(packageProjects, filePath, writeFullPath);
    }
}