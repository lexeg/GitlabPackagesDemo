using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GitlabPackagesDemo.Annotations;
using GitlabPackagesDemo.Commands;
using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.GitLab;
using GitlabPackagesDemo.Settings;
using GitlabPackagesDemo.Views;
using Microsoft.Extensions.Options;
using Microsoft.Win32;

namespace GitlabPackagesDemo.ViewModels;

public class RepositoriesViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private readonly GitLabSettings _settings;
    private readonly FileSaver _fileSaver;
    private readonly RepositoryService _repositoryService;
    private readonly SettingsDialog _settingsDialog;
    private GitRepository[] _repositories;


    public RepositoriesViewModel(Window window,
        IOptions<GitLabSettings> settings,
        FileSaver fileSaver,
        RepositoryService repositoryService,
        SettingsDialog settingsDialog)
    {
        _window = window;
        _settings = settings.Value;
        _fileSaver = fileSaver;
        _repositoryService = repositoryService;
        _settingsDialog = settingsDialog;
        InitializeCommands();
    }

    public ICommand ShowRepositoriesCommand { get; private set; }
    
    public ICommand ClickButtonCommand { get; private set; }
    
    public ICommand OpenSettingsCommand { get; private set; }
    
    public ICommand SaveRepositoriesCommand { get; private set; }

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
        CloseAppCommand = new BaseAutoEventCommand(_ => _window.Close(), _ => true);
        SaveRepositoriesCommand = new BaseAutoEventCommand(async _ =>
        {
            var saveFileDialog = new SaveFileDialog
                { DefaultExt = "*.txt", Filter = "Текстовые документы |*.txt", FileName = "projects.txt" };
            if (saveFileDialog.ShowDialog() != true) return;
            var filePath = saveFileDialog.FileName;
            await _fileSaver.SaveProjects(filePath, _repositories);
        }, _ => HasData);
    }

    private async void LoadRepositories()
    {
        using var client = new GitLabClient(_settings);
        Repositories = await GetAllProjects(client);
    }
    
    private async void ButtonBase_OnClick()
    {
        const string rootDirectory = "prjs";
        using var client = new GitLabClient(_settings);
        Repositories ??= await GetAllProjects(client);
        var filesInProject = await _repositoryService.GetFilesInProject(client, "PackageReference", "csproj", Repositories, rootDirectory);
        var filesContent = await _repositoryService.GetFilesContent(client, filesInProject, rootDirectory);
        var result = _repositoryService.GroupToPackageProjects(filesContent);
        await _fileSaver.Serialize(rootDirectory, result);
        await _fileSaver.CreateList(rootDirectory, result, "list.txt", true);
        await _fileSaver.CreateList(rootDirectory, result, "list2.txt", false);
    }
    
    private void OpenSettings()
    {
        var gitlabSettings = _settings;
        if (_settingsDialog.ShowDialog().GetValueOrDefault())
        {
            if (_settingsDialog.DataContext is not SettingsViewModel settingsDialogDataContext) return;
            gitlabSettings.Host = settingsDialogDataContext.Host;
            gitlabSettings.PrivateToken = settingsDialogDataContext.Token;
        }
    }

    private async Task<GitRepository[]> GetAllProjects(GitLabClient client) => await client.GetProjects();
}