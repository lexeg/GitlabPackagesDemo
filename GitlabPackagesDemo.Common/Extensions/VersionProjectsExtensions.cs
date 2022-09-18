using GitlabPackagesDemo.Common.Data;

namespace GitlabPackagesDemo.Common.Extensions;

public static class VersionProjectsExtensions
{
    public static IEnumerable<string> GetProjectsNames(this VersionProjects versionProjects, bool isWriteFullPath) =>
        isWriteFullPath
            ? versionProjects.Projects
            : versionProjects.Projects.Select(x => x.Split('/').Last());
}