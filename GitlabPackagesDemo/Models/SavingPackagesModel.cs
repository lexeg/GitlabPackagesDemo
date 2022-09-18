using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GitlabPackagesDemo.Models;

public class SavingPackagesModel : INotifyPropertyChanged
{
    private string _folderPath;
    private string _fileName;
    private bool _writeFullPath;

    public string FolderPath
    {
        get => _folderPath;
        set
        {
            _folderPath = value;
            OnPropertyChanged(nameof(FolderPath));
        }
    }

    public string FileName
    {
        get => _fileName;
        set
        {
            _fileName = value;
            OnPropertyChanged(nameof(FileName));
        }
    }

    public bool WriteFullPath
    {
        get => _writeFullPath;
        set
        {
            _writeFullPath = value;
            OnPropertyChanged(nameof(WriteFullPath));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}