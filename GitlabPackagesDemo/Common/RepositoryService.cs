using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using GitlabPackagesDemo.Comparers;
using GitlabPackagesDemo.GitLab;
using GitlabPackagesDemo.Settings;

namespace GitlabPackagesDemo.Common;

public class RepositoryService
{
    public async Task<RepoFiles[]> GetFilesInProject(GitLabClient client,
        SearchSettings searchSettings,
        GitRepository[] repositories)
    {
        var searchText = searchSettings.SearchText;
        var fileExtension = searchSettings.FileExtension;
        var repoFiles = new List<RepoFiles>();
        foreach (var item in repositories)
        {
            var filesInProject = await client.SearchFilesInProject(item.Id, searchText, fileExtension);
            repoFiles.Add(new RepoFiles { Repository = item, Files = filesInProject });
        }

        return repoFiles.ToArray();
    }
    
    public async Task<PackageData[]> GetFilesContent(GitLabClient client, RepoFiles[] repoFiles)
    {
        var packageDataItems = new List<PackageData>();
        foreach (var repoFile in repoFiles)
        {
            var packages = new List<PackageReference>();
            foreach (var repoFileFile in repoFile.Files)
            {
                var fileByName = await client.GetFileByName(repoFile.Repository.Id,
                    repoFileFile.FileName,
                    repoFileFile.Ref);
                var items = GetPackages(fileByName);
                packages.AddRange(items);
            }

            packageDataItems.AddRange(packages.Select(reference => new PackageData
            {
                Package = reference.Include,
                Project = repoFile.Repository.PathWithNamespace,
                Version = reference.Version
            }));
        }

        return packageDataItems.ToArray();
    }
    
    private PackageReference[] GetPackages(string fileContent)
    {
        // Debug.WriteLine(fileName);
        var packageReferences = new List<PackageReference>();
        var doc = new XmlDocument();
        // doc.Load(fileName);
        doc.LoadXml(fileContent);

        XmlNode root = doc.DocumentElement;
        var nodeList = root?.SelectNodes("descendant::ItemGroup");
        foreach (XmlNode book in nodeList)
        {
            foreach (var node in book.ChildNodes.OfType<XmlElement>().Where(n =>
                         n.Name.Equals("PackageReference", StringComparison.InvariantCultureIgnoreCase) ||
                         n.Name.Equals("ProjectReference", StringComparison.InvariantCultureIgnoreCase)))
            {
                var pr = new PackageReference();
                if (node.HasAttribute("Include")) pr.Include = node.GetAttribute("Include");
                if (node.HasAttribute("Version")) pr.Version = node.GetAttribute("Version");
                packageReferences.Add(pr);
            }
        }

        return packageReferences.ToArray();
    }

    public PackageProjects[] GroupToPackageProjects(PackageData[] packageDataItems)
    {
        var func2 =
            new Func<IGrouping<string, PackageData>,
                IEnumerable<ProjectData>>(x => x.Select(xx => new ProjectData
            {
                Project = xx.Project,
                Version = xx.Version
            }));

        var func =
            new Func<IGrouping<string, PackageData>, ProjectData[]>(x =>
                func2(x).Distinct(new ProjectWithVersionComparer()).ToArray());

        var result = packageDataItems
            .GroupBy(fc => fc.Package)
            .ToDictionary(x => x.Key, func)
            .Select(x => new PackageProjects
            {
                Package = x.Key,
                Projects = x.Value
            })
            .ToArray();

        return result;
    }
}