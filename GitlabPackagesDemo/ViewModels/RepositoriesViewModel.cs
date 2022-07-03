using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using GitlabPackagesDemo.Annotations;
using GitlabPackagesDemo.Commands;
using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.Comparers;
using GitlabPackagesDemo.GitLab;
using GitlabPackagesDemo.Settings;
using GitlabPackagesDemo.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace GitlabPackagesDemo.ViewModels;

public class RepositoriesViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private readonly FileSaver _fileSaver;
    private Root[] _repositories;


    public RepositoriesViewModel(Window window, FileSaver fileSaver)
    {
        _window = window;
        _fileSaver = fileSaver;
        InitializeCommands();
    }

    public ICommand ShowRepositoriesCommand { get; private set; }
    
    public ICommand ClickButtonCommand { get; private set; }
    
    public ICommand OpenSettingsCommand { get; private set; }
    
    public ICommand SaveRepositoriesCommand { get; private set; }

    public Root[] Repositories
    {
        get => _repositories;
        set
        {
            _repositories = value;
            OnPropertyChanged(nameof(Repositories));
            OnPropertyChanged(nameof(HasData));
        }
    }

    public ICommand CloseAppCommand { get; private set; }

    public bool HasData => _repositories != null && _repositories.Any();
    
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void InitializeCommands()
    {
        ShowRepositoriesCommand = new BaseAutoEventCommand(_ => LoadRepositories(), _ => true);
        ClickButtonCommand = new BaseAutoEventCommand(_ => ButtonBase_OnClick(), _ => HasData);
        OpenSettingsCommand = new BaseAutoEventCommand(_ => OpenSettings(), _ => true);
        CloseAppCommand = new BaseAutoEventCommand(_ => _window.Close(), o => true);
        SaveRepositoriesCommand = new BaseAutoEventCommand(async _ =>
        {
            var saveFileDialog = new SaveFileDialog
                { DefaultExt = "*.txt", Filter = "Текстовые документы |*.txt", FileName = "projects.txt" };
            if (saveFileDialog.ShowDialog() != true) return;
            var filePath = saveFileDialog.FileName;
            await _fileSaver.SaveProjects(filePath, _repositories);
        }, _ => HasData);
    }

    private GitLabSettings GetSettings() => new()
    {
        Host = "https://gitlab-orms.ad.speechpro.com",
        PrivateToken = "D2tpy9ph-guGKfeVnemn"
    };

    private async void LoadRepositories()
    {
        var settings = GetSettings();
        using var client = new GitLabClient(settings);
        Repositories = await GetAllProjects(client);
    }
    
    private async void ButtonBase_OnClick()
    {
        const string rootDirectory = "prjs";
        var settings = GetSettings();
        using var client = new GitLabClient(settings);
        Repositories ??= await GetAllProjects(client);
        var filesInProject = await GetFilesInProject(client, "PackageReference", "csproj", Repositories, rootDirectory);
        var filesContent = await GetFilesContent(client, filesInProject, rootDirectory);
        var result = GroupToDictionary(filesContent);
        var serializeObject = JsonConvert.SerializeObject(result, Formatting.Indented);
        var directoryInfo = Directory.CreateDirectory(rootDirectory);
        await File.WriteAllTextAsync(Path.Combine(directoryInfo.FullName, "packages.json"), serializeObject);
        await CreateList(directoryInfo, result, "list.txt", true);
        await CreateList(directoryInfo, result, "list2.txt", false);
    }
    
    private void OpenSettings()
    {
        var gitlabSettings = GetSettings();
        var settingsDialog = new SettingsDialog(gitlabSettings);
        if (settingsDialog.ShowDialog().GetValueOrDefault())
        {
            if (settingsDialog.DataContext is not SettingsViewModel settingsDialogDataContext) return;
            gitlabSettings.Host = settingsDialogDataContext.Host;
            gitlabSettings.PrivateToken = settingsDialogDataContext.Token;
        }
    }

    private async Task<Root[]> GetAllProjects(GitLabClient client) => await client.GetProjects();
    
    private async Task<RepoFiles[]> GetFilesInProject(GitLabClient client,
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
            var dir = Path.Combine(rootDirectory, $"{item.Name}-{item.Id}");
            if (Directory.Exists(dir)) Directory.Delete(dir);
            var directoryInfo = Directory.CreateDirectory(dir);
            var builder = new StringBuilder();
            foreach (var f in filesInProject)
            {
                builder.AppendLine($"{f.ProjectId}; {f.FileName}; {f.Path}; {f.Ref}");
            }

            await File.WriteAllTextAsync(Path.Combine(directoryInfo.FullName, "res.txt"), builder.ToString());
        }

        return repoFiles.ToArray();
    }
    
    private async Task<(string Package, string Project, string Version)[]> GetFilesContent(GitLabClient client,
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
                var path = Path.Combine(directoryInfo.FullName, last);
                await File.WriteAllTextAsync(path, fileByName);
                var items = GetPackages(path);
                packages.AddRange(items);
            }

            tuples.AddRange(packages.Select(reference =>
                (reference.Include, path_with_namespace: repoFile.Repository.PathWithNamespace, reference.Version)));
        }

        return tuples.ToArray();
    }
    
    private PackageReference[] GetPackages(string fileName)
    {
        Debug.WriteLine(fileName);
        var packageReferences = new List<PackageReference>();
        var doc = new XmlDocument();
        doc.Load(fileName);

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
    
    private async Task CreateList(DirectoryInfo directoryInfo,
        KeyValuePair<string, (string Project, string Version)[]>[] res,
        string fileName,
        bool foolPath)
    {
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
    
    private KeyValuePair<string, (string Project, string Version)[]>[] GroupToDictionary(
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