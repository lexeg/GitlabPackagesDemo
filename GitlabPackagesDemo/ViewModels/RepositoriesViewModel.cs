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
using Microsoft.Win32;

namespace GitlabPackagesDemo.ViewModels;

public class RepositoriesViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private readonly FileSaver _fileSaver;
    private readonly RepositoryService _repositoryService;
    private Root[] _repositories;


    public RepositoriesViewModel(Window window, FileSaver fileSaver, RepositoryService repositoryService)
    {
        _window = window;
        _fileSaver = fileSaver;
        _repositoryService = repositoryService;
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
        var filesInProject = await _repositoryService.GetFilesInProject(client, "PackageReference", "csproj", Repositories, rootDirectory, _fileSaver);
        var filesContent = await _repositoryService.GetFilesContent(client, filesInProject, rootDirectory, _fileSaver);
        var result = _repositoryService.GroupToDictionary(filesContent);
        await _fileSaver.Serialize(rootDirectory, result);
        await _fileSaver.CreateList(rootDirectory, result, "list.txt", true);
        await _fileSaver.CreateList(rootDirectory, result, "list2.txt", false);
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
}