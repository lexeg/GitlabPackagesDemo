using System.ComponentModel;
using System.Runtime.CompilerServices;
using GitlabPackagesDemo.Annotations;

namespace GitlabPackagesDemo.Models;

public class SearchSettingsModel : INotifyPropertyChanged
{
    private string _searchText;
    private string _fileExtension;

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
        }
    }

    public string FileExtension
    {
        get => _fileExtension;
        set
        {
            _fileExtension = value;
            OnPropertyChanged(nameof(FileExtension));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}