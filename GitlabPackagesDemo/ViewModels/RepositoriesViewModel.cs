using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GitlabPackagesDemo.Annotations;
using GitlabPackagesDemo.Commands;
using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.GitLab;
using GitlabPackagesDemo.Helpers;
using GitlabPackagesDemo.Settings;
using Microsoft.Extensions.Options;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace GitlabPackagesDemo.ViewModels;

public class RepositoriesViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private readonly IOptions<GitLabSettings> _gitLabSettings;
    private readonly IOptions<SearchSettings> _searchSettings;
    private readonly FileSaver _fileSaver;
    private readonly RepositoryService _repositoryService;
    private readonly DialogFactory _dialogFactory;
    private GitRepository[] _repositories;
    private PackageProjects[] _packageProjects;
    private RepoFiles[] _filesInProject;


    public RepositoriesViewModel(Window window,
        IOptions<GitLabSettings> gitLabSettings,
        IOptions<SearchSettings> searchSettings,
        FileSaver fileSaver,
        RepositoryService repositoryService,
        DialogFactory dialogFactory)
    {
        _window = window;
        _gitLabSettings = gitLabSettings;
        _searchSettings = searchSettings;
        _fileSaver = fileSaver;
        _repositoryService = repositoryService;
        _dialogFactory = dialogFactory;
        InitializeCommands();
    }

    public ICommand ShowRepositoriesCommand { get; private set; }
    
    public ICommand LoadPackageProjectsCommand { get; private set; }
    
    public ICommand FindFilesInProjectCommand { get; private set; }
    
    public ICommand OpenSettingsCommand { get; private set; }
    
    public ICommand SaveRepositoriesCommand { get; private set; }

    public ICommand SavePackagesCommand { get; private set; }

    public GitRepository[] Repositories
    {
        get => _repositories;
        set
        {
            _repositories = value;
            OnPropertyChanged(nameof(Repositories));
            OnPropertyChanged(nameof(HasData));
        }
    }

    public PackageProjects[] PackageProjects
    {
        get => _packageProjects;
        set
        {
            _packageProjects = value;
            OnPropertyChanged(nameof(PackageProjects));
        }
    }

    public RepoFiles[] FilesInProject
    {
        get => _filesInProject;
        set
        {
            _filesInProject = value;
            OnPropertyChanged(nameof(FilesInProject));
        }
    }

    public ICommand CloseAppCommand { get; private set; }

    public bool HasData => _repositories != null && _repositories.Any();

    public bool HasPackagesData => _packageProjects != null && _packageProjects.Any();
    
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void InitializeCommands()
    {
        ShowRepositoriesCommand = new BaseAutoEventCommand(_ => LoadRepositories(), _ => true);
        LoadPackageProjectsCommand = new BaseAutoEventCommand(_ => LoadPackageProjects(), _ => HasData);
        FindFilesInProjectCommand = new BaseAutoEventCommand(_ => FindFilesInProject(), _ => HasData);
        OpenSettingsCommand = new BaseAutoEventCommand(_ => OpenSettings(), _ => true);
        CloseAppCommand = new BaseAutoEventCommand(_ => _window.Close(), _ => true);
        SaveRepositoriesCommand = new BaseAutoEventCommand(async _ =>
        {
            var saveFileDialog = new SaveFileDialog
                { DefaultExt = "*.txt", Filter = "Текстовые документы |*.txt", FileName = "projects.txt" };
            if (saveFileDialog.ShowDialog() != true) return;
            var filePath = saveFileDialog.FileName;
            await _fileSaver.SaveProjects(filePath, _repositories);
        }, _ => HasData);
        SavePackagesCommand = new BaseAutoEventCommand(async _ =>
        {
            var folderBrowDialog = new FolderBrowserDialog { ShowNewFolderButton = true };
            if (folderBrowDialog.ShowDialog() != DialogResult.OK) return;
            var folderPath = folderBrowDialog.SelectedPath;
            await _fileSaver.Serialize(folderPath, PackageProjects);
            await _fileSaver.CreateList(folderPath, PackageProjects, "list.txt", true);
            await _fileSaver.CreateList(folderPath, PackageProjects, "list2.txt", false);
        }, _ => HasPackagesData);
    }

    private async void LoadRepositories()
    {
        using var client = new GitLabClient(_gitLabSettings.Value);
        Repositories = await GetAllProjects(client);
    }
    
    private async void LoadPackageProjects()
    {
        const string rootDirectory = "prjs";
        var settings = _searchSettings.Value;
        using var client = new GitLabClient(_gitLabSettings.Value);
        Repositories ??= await GetAllProjects(client);
        FilesInProject ??= await _repositoryService.GetFilesInProject(client, settings.SearchText, settings.FileExtension, Repositories, rootDirectory);
        var filesContent = await _repositoryService.GetFilesContent(client, FilesInProject, rootDirectory);
        PackageProjects = _repositoryService.GroupToPackageProjects(filesContent);
    }
    
    private async void FindFilesInProject()
    {
        const string rootDirectory = "prjs";
        var searchSettings = _searchSettings.Value;
        var searchDialog = _dialogFactory.CreateSearchDialog(_searchSettings);
        if (searchDialog.ShowDialog().GetValueOrDefault())
        {
            if (searchDialog.DataContext is not SearchViewModel searchDialogDataContext) return;
            searchSettings.SearchText = searchDialogDataContext.SearchText;
            searchSettings.FileExtension = searchDialogDataContext.FileExtension;
            using var client = new GitLabClient(_gitLabSettings.Value);
            FilesInProject ??= await _repositoryService.GetFilesInProject(client, searchSettings.SearchText, searchSettings.FileExtension, Repositories, rootDirectory);
            MessageBox.Show("Done");
        }
    }
    
    private void OpenSettings()
    {
        var gitlabSettings = _gitLabSettings.Value;
        var settingsDialog = _dialogFactory.CreateSettingsDialog(_gitLabSettings);
        if (settingsDialog.ShowDialog().GetValueOrDefault())
        {
            if (settingsDialog.DataContext is not SettingsViewModel settingsDialogDataContext) return;
            gitlabSettings.Host = settingsDialogDataContext.Host;
            gitlabSettings.PrivateToken = settingsDialogDataContext.Token;
        }
    }

    private async Task<GitRepository[]> GetAllProjects(GitLabClient client) => await client.GetProjects();
}