using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GitlabPackagesDemo.Commands;
using GitlabPackagesDemo.Common.Settings;
using GitlabPackagesDemo.Models;
using Microsoft.Extensions.Options;

namespace GitlabPackagesDemo.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private readonly GitLabSettings _settings;
    private SettingsModel _currentSettings;

    public SettingsViewModel(Window window, IOptions<GitLabSettings> settings)
    {
        _window = window;
        _settings = settings.Value;
        _currentSettings = CreateSettingsModel(settings.Value);
        InitializeCommands();
    }

    public string Host
    {
        get => _currentSettings.Host;
        set
        {
            _currentSettings.Host = value;
            OnPropertyChanged(nameof(Host));
        }
    }

    public string Token
    {
        get => _currentSettings.Token;
        set
        {
            _currentSettings.Token = value;
            OnPropertyChanged(nameof(Token));
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