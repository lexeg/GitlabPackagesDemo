using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GitlabPackagesDemo.Annotations;
using GitlabPackagesDemo.Commands;
using GitlabPackagesDemo.Models;
using GitlabPackagesDemo.Settings;

namespace GitlabPackagesDemo.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private readonly GitLabSettings _settings;
    private SettingsModel _currentSettings;

    public SettingsViewModel(Window window, GitLabSettings settings)
    {
        _window = window;
        _settings = settings;
        _currentSettings = CreateSettingsModel(settings);
        InitializeCommands();
    }

    public SettingsModel CurrentSettings
    {
        get => _currentSettings;
        set
        {
            _currentSettings = value;
            OnPropertyChanged(nameof(CurrentSettings));
        }
    }

    public ICommand ApplyCommand { get; private set; }

    public ICommand CancelCommand { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void InitializeCommands()
    {
        ApplyCommand = new BaseAutoEventCommand(_ => _window.DialogResult = true, _ => true);
        CancelCommand = new BaseAutoEventCommand(_ =>
        {
            _currentSettings = CreateSettingsModel(_settings);
            _window.DialogResult = false;
        }, _ => true);
    }

    private SettingsModel CreateSettingsModel(GitLabSettings settings) => new()
    {
        Host = settings.Host,
        Token = settings.PrivateToken
    };
}