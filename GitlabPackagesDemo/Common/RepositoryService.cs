using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using GitlabPackagesDemo.Comparers;
using GitlabPackagesDemo.GitLab;

namespace GitlabPackagesDemo.Common;

public class RepositoryService
{
    private readonly FileSaver _fileSaver;

    public RepositoryService(FileSaver fileSaver)
    {
        _fileSaver = fileSaver;
    }
    
    public async Task<RepoFiles[]> GetFilesInProject(GitLabClient client,
        string searchText,
        string fileExtension,
        Root[] items,
        string rootDirectory)
    {
        var repoFiles = new List<RepoFiles>();
        foreach (var item in items)
        {
            var filesInProject = await client.SearchFilesInProject(item.Id, searchText, fileExtension);
            repoFiles.Add(new RepoFiles { Repository = item, Files = filesInProject });
            await _fileSaver.SaveProjectFiles(filesInProject, rootDirectory, $"{item.Name}-{item.Id}");
        }

        return repoFiles.ToArray();
    }
    
    public async Task<(string Package, string Project, string Version)[]> GetFilesContent(GitLabClient client,
        RepoFiles[] repoFiles,
        string rootDirectory)
    {
        var tuples = new List<(string Package, string Project, string Version)>();
        foreach (var repoFile in repoFiles)
        {
            var dir = Path.Combine(rootDirectory, $"{repoFile.Repository.Name}-{repoFile.Repository.Id}");
            var directoryInfo = Directory.CreateDirectory(dir);
            var packages = new List<PackageReference>();
            foreach (var repoFileFile in repoFile.Files)
            {
                var fileByName = await client.GetFileByName(repoFile.Repository.Id, repoFileFile.FileName,
                    repoFileFile.Ref);
                var last = repoFileFile.FileName.Split('/').Last();
                await _fileSaver.SaveFileContent(fileByName, directoryInfo, last);
                var items = GetPackages(fileByName);
                packages.AddRange(items);
            }

            tuples.AddRange(packages.Select(reference =>
                (reference.Include, path_with_namespace: repoFile.Repository.PathWithNamespace, reference.Version)));
        }

        return tuples.ToArray();
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
    
    public KeyValuePair<string, (string Project, string Version)[]>[] GroupToDictionary(
        (string Package, string Project, string Version)[] filesContent)
    {
        var func2 =
            new Func<IGrouping<string, (string Package, string Project, string Version)>,
                IEnumerable<(string Project, string Version)>>(x => x.Select(xx => (xx.Project, xx.Version)));

        var func =
            new Func<IGrouping<string, (string Package, string Project, string Version)>, (string Project, string
                Version)[]>(x => func2(x).Distinct(new ProjectWithVersionComparer()).ToArray());

        var result = filesContent
            .GroupBy(fc => fc.Package)
            .ToDictionary(x => x.Key, func)
            .ToArray();
        return result;
    }
}