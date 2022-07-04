using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.Settings;
using GitlabPackagesDemo.ViewModels;
using Microsoft.Extensions.Options;

namespace GitlabPackagesDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(IOptions<GitLabSettings> settings,
            FileSaver fileSaver,
            RepositoryService repositoryService,
            SettingsDialog settingsDialog)
        {
            InitializeComponent();
            DataContext = new RepositoriesViewModel(this, settings, fileSaver, repositoryService, settingsDialog);
        }
    }
}