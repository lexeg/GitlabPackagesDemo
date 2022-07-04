using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitlabPackagesDemo.GitLab;
using Newtonsoft.Json;

namespace GitlabPackagesDemo.Common;

public class FileSaver
{
    public async Task SaveProjects(string filePath, Root[] repositories)
    {
        var builder = new StringBuilder();
        foreach (var repository in repositories)
        {
            builder.AppendLine(
                $"{repository.Id}; {repository.Name}; {repository.PathWithNamespace}; {repository.WebUrl}");
        }

        await File.WriteAllTextAsync(filePath, builder.ToString());
    }

    public async Task SaveProjectFiles(RootFile[] filesInProject, string rootDirectory, string subDir)
    {
        var dir = Path.Combine(rootDirectory, subDir);
        if (Directory.Exists(dir)) Directory.Delete(dir);
        var directoryInfo = Directory.CreateDirectory(dir);
        var builder = new StringBuilder();
        foreach (var f in filesInProject)
        {
            builder.AppendLine($"{f.ProjectId}; {f.FileName}; {f.Path}; {f.Ref}");
        }

        await File.WriteAllTextAsync(Path.Combine(directoryInfo.FullName, "res.txt"), builder.ToString());
    }

    public async Task SaveFileContent(string fileContent, DirectoryInfo directoryInfo, string last)
    {
        var path = Path.Combine(directoryInfo.FullName, last);
        await File.WriteAllTextAsync(path, fileContent);
    }

    public async Task Serialize(string directoryPath, KeyValuePair<string, (string Project, string Version)[]>[] result)
    {
        var directoryInfo = Directory.CreateDirectory(directoryPath);
        var serializeObject = JsonConvert.SerializeObject(result, Formatting.Indented);
        await File.WriteAllTextAsync(Path.Combine(directoryInfo.FullName, "packages.json"), serializeObject);
    }
    
    public async Task CreateList(string directoryPath,
        KeyValuePair<string, (string Project, string Version)[]>[] res,
        string fileName,
        bool foolPath)
    {
        var directoryInfo = Directory.CreateDirectory(directoryPath);
        var content = new StringBuilder();
        foreach (var (key, value) in res)
        {
            var sb = new StringBuilder();
            var stringsMap = value
                .Where(v => !string.IsNullOrEmpty(v.Version))
                .OrderBy(v => v.Version)
                .GroupBy(v => v.Version, v => v.Project)
                .ToDictionary(v => v.Key, v => v.ToArray());
            foreach (var (key1, value1) in stringsMap)
            {
                var projects = foolPath ? value1 : value1.Select(x => x.Split('/').Last());
                var join = string.Join(", ", projects);
                sb.AppendLine($"\t{key1} ({join})");
            }

            if (sb.Length != 0)
            {
                content.AppendLine($"{key}:");
                content.AppendLine(sb.ToString());
            }
        }

        await File.WriteAllTextAsync(Path.Combine(directoryInfo.FullName, fileName), content.ToString());
    }
}