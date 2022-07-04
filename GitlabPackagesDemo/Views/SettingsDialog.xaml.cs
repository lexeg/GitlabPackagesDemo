using GitlabPackagesDemo.Settings;
using GitlabPackagesDemo.ViewModels;
using Microsoft.Extensions.Options;

namespace GitlabPackagesDemo.Views;

public partial class SettingsDialog
{
    public SettingsDialog(IOptions<GitLabSettings> settings)
    {
        InitializeComponent();
        DataContext = new SettingsViewModel(this, settings);
    }
}