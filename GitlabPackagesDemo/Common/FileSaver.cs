using System.IO;
using System.Text;
using System.Threading.Tasks;
using GitlabPackagesDemo.GitLab;

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
}