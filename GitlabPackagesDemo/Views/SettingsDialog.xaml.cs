using GitlabPackagesDemo.Settings;
using GitlabPackagesDemo.ViewModels;

namespace GitlabPackagesDemo.Views;

public partial class SettingsDialog
{
    public SettingsDialog(GitLabSettings settings)
    {
        InitializeComponent();
        DataContext = new SettingsViewModel(this, settings);
    }
}