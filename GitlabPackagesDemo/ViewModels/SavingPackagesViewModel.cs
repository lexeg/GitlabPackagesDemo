using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GitlabPackagesDemo.Commands;
using GitlabPackagesDemo.Models;

namespace GitlabPackagesDemo.ViewModels;

public class SavingPackagesViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private SavingPackagesModel _currentSavingPackagesModel;

    public SavingPackagesViewModel(Window window)
    {
        _window = window;
        _currentSavingPackagesModel = CreateSavingPackagesModel();
        InitializeCommands();
    }

    public string FolderPath
    {
        get => _currentSavingPackagesModel.FolderPath;
        set
        {
            _currentSavingPackagesModel.FolderPath = value;
            OnPropertyChanged(nameof(FolderPath));
        }
    }

    public string FileName
    {
        get => _currentSavingPackagesModel.FileName;
        set
        {
            _currentSavingPackagesModel.FileName = value;
            OnPropertyChanged(nameof(FileName));
        }
    }

    public bool WriteFullPath
    {
        get => _currentSavingPackagesModel.WriteFullPath;
        set
        {
            _currentSavingPackagesModel.WriteFullPath = value;
            OnPropertyChanged(nameof(WriteFullPath));
        }
    }

    public ICommand OkCommand { get; private set; }

    public ICommand CancelCommand { get; private set; }

    public ICommand OpenFolderDialogCommand { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void InitializeCommands()
    {
        OkCommand = new BaseAutoEventCommand(_ => _window.DialogResult = true,
            _ => !string.IsNullOrEmpty(FolderPath) && !string.IsNullOrEmpty(FileName));
        CancelCommand = new BaseAutoEventCommand(_ =>
        {
            _currentSavingPackagesModel = CreateSavingPackagesModel();
            _window.DialogResult = false;
        }, _ => true);
        OpenFolderDialogCommand = new BaseAutoEventCommand(_ =>
        {
            var folderBrowserDialog = new FolderBrowserDialog { ShowNewFolderButton = true };
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;
            FolderPath = folderBrowserDialog.SelectedPath;
        }, _ => true);
    }

    private SavingPackagesModel CreateSavingPackagesModel() => new()
    {
        FolderPath = string.Empty,
        FileName = "package-projects.txt",
        WriteFullPath = false
    };
}