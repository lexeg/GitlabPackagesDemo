using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GitlabPackagesDemo.Models;

public class SettingsModel : INotifyPropertyChanged
{
    private string _host;
    private string _token;

    public string Host
    {
        get => _host;
        set
        {
            _host = value;
            OnPropertyChanged(nameof(Host));
        }
    }

    public string Token
    {
        get => _token;
        set
        {
            _token = value;
            OnPropertyChanged(nameof(Token));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}