using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GitlabPackagesDemo.Annotations;
using GitlabPackagesDemo.Commands;
using GitlabPackagesDemo.Settings;
using Microsoft.Extensions.Options;

namespace GitlabPackagesDemo.ViewModels;

public class SearchViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private readonly SearchSettings _settings;
    private SearchSettings _currentSettings;

    public SearchViewModel(Window window, IOptions<SearchSettings> settings)
    {
        _window = window;
        _settings = settings.Value;
        _currentSettings = CreateSettingsModel(settings.Value);
        InitializeCommands();
    }

    public string SearchText
    {
        get => _currentSettings.SearchText;
        set
        {
            _currentSettings.SearchText = value;
            OnPropertyChanged(nameof(SearchText));
        }
    }
    
    public string FileExtension
    {
        get => _currentSettings.FileExtension;
        set
        {
            _currentSettings.FileExtension = value;
            OnPropertyChanged(nameof(FileExtension));
        }
    }
    
    public ICommand FindCommand { get; private set; }
    
    public ICommand CancelCommand { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void InitializeCommands()
    {
        FindCommand = new BaseAutoEventCommand(_ => _window.DialogResult = true, _ => true);
        CancelCommand = new BaseAutoEventCommand(_ =>
        {
            _currentSettings = CreateSettingsModel(_settings);
            _window.DialogResult = false;
        }, _ => true);
    }

    private SearchSettings CreateSettingsModel(SearchSettings settings) => new()
    {
        SearchText = settings.SearchText,
        FileExtension = settings.FileExtension
    };
}