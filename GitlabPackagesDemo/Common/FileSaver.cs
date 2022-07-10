using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitlabPackagesDemo.GitLab;
using Newtonsoft.Json;

namespace GitlabPackagesDemo.Common;

public class FileSaver
{
    public async Task SaveProjects(GitRepository[] repositories, string filePath)
    {
        var builder = new StringBuilder();
        foreach (var repository in repositories)
        {
            builder.AppendLine(
                $"{repository.Id}; {repository.Name}; {repository.PathWithNamespace}; {repository.WebUrl}");
        }

        await File.WriteAllTextAsync(filePath, builder.ToString());
    }

    public async Task SaveProjectFiles(RepositoryFileData[] repositoryFiles, string filePath)
    {
        var directoryPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directoryPath)) return;
        if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
        var builder = new StringBuilder();
        foreach (var f in repositoryFiles)
        {
            builder.AppendLine($"{f.ProjectId}; {f.FileName}; {f.Path}; {f.Ref}");
        }

        await File.WriteAllTextAsync(filePath, builder.ToString());
    }

    public Task SaveFileContent(string filePath, string fileContent) => File.WriteAllTextAsync(filePath, fileContent);

    public async Task Serialize(PackageProjects[] packageItems, string filePath)
    {
        var directoryPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directoryPath)) return;
        var serializeObject = JsonConvert.SerializeObject(packageItems, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, serializeObject);
    }

    public async Task CreatePackagesFile(PackageProjects[] packageItems, string filePath, bool writeFullPath)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directoryName)) return;
        Directory.CreateDirectory(directoryName);
        var content = new StringBuilder();
        foreach (var packageItem in packageItems)
        {
            var sb = new StringBuilder();
            var stringsMap = packageItem.Projects
                .Where(v => !string.IsNullOrEmpty(v.Version))
                .OrderBy(v => v.Version)
                .GroupBy(v => v.Version, v => v.Project)
                .ToDictionary(v => v.Key, v => v.ToArray());
            foreach (var (key1, value1) in stringsMap)
            {
                var projects = writeFullPath ? value1 : value1.Select(x => x.Split('/').Last());
                var join = string.Join(", ", projects);
                sb.AppendLine($"\t{key1} ({join})");
            }

            if (sb.Length != 0)
            {
                content.AppendLine($"{packageItem.Package}:");
                content.AppendLine(sb.ToString());
            }
        }

        await File.WriteAllTextAsync(filePath, content.ToString());
    }
}