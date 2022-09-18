using GitlabPackagesDemo.Common.Comparers;
using GitlabPackagesDemo.Common.Data;

namespace GitlabPackagesDemo.Common.Extensions;

public static class PackageDataExtensions
{
    public static PackageProjects[] GetPackageProjects(this PackageData[] packageDataItems)
    {
        var items = new List<PackageProjects>();
        var dictionary = packageDataItems
            .GroupBy(x => x.Package)
            .ToDictionary(x => x.Key, x => x.ToArray());
        foreach (var (package, values) in dictionary)
        {
            items.Add(CreatePackage(package, values));
        }

        return items.ToArray();
    }

    public static PackageWithVersionProjects[] GetPackageWithVersionProjects(this PackageData[] packageDataItems)
    {
        var items = new List<PackageWithVersionProjects>();
        var dictionary = packageDataItems.GroupBy(x => x.Package)
            .ToDictionary(x => x.Key, x => x.ToArray());
        foreach (var (package, values) in dictionary)
        {
            items.Add(new PackageWithVersionProjects
            {
                Package = package,
                VersionProjects = CreateVersionProjects(values)
            });
        }

        return items.ToArray();
    }

    private static PackageProjects CreatePackage(string package, PackageData[] items) => new()
    {
        Package = package,
        Projects = items.Select(x => new ProjectData
            {
                Project = x.Project,
                Version = x.Version
            })
            .Distinct(new ProjectDataComparer())
            .ToArray()
    };

    private static VersionProjects[] CreateVersionProjects(PackageData[] packageDataItems)
    {
        var items = new List<VersionProjects>();
        var dictionary = packageDataItems.GroupBy(x => x.Version)
            .ToDictionary(x => x.Key ?? "No version", x => x.ToArray());
        foreach (var (version, values) in dictionary)
        {
            items.Add(new VersionProjects
            {
                Version = version,
                Projects = values.Select(x => x.Project).Distinct().ToArray()
            });
        }

        return items.OrderBy(x => x.Version).ToArray();
    }
}