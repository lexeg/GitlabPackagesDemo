using GitlabPackagesDemo.Common.Data;

namespace GitlabPackagesDemo.Common.Extensions;

public static class PackageProjectsExtensions
{
    public static PackageWithVersionProjects[] GetPackageWithVersionProjects(this PackageProjects[] packageItems) =>
        packageItems.Select(CreatePackageWithVersionProjects).ToArray();

    private static PackageWithVersionProjects CreatePackageWithVersionProjects(PackageProjects packageProjects) => new()
        { Package = packageProjects.Package, VersionProjects = CreateVersionProjects(packageProjects.Projects) };

    private static VersionProjects[] CreateVersionProjects(ProjectData[] projectDataItems)
    {
        var dictionary = projectDataItems
            .OrderBy(v => v.Version)
            .GroupBy(v => v.Version, v => v.Project)
            .ToDictionary(v => v.Key ?? "No version", v => v.ToArray());
        var versionProjects = new List<VersionProjects>();
        foreach (var (version, projects) in dictionary)
        {
            versionProjects.Add(new VersionProjects { Version = version, Projects = projects });
        }

        return versionProjects.ToArray();
    }
}