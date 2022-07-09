using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.Helpers;
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
        public MainWindow(IOptions<GitLabSettings> gitLabSettings,
            IOptions<SearchSettings> searchSettings,
            FileSaver fileSaver,
            RepositoryService repositoryService,
            DialogFactory dialogFactory)
        {
            InitializeComponent();
            DataContext = new RepositoriesViewModel(this,
                gitLabSettings,
                searchSettings,
                fileSaver,
                repositoryService,
                dialogFactory);
        }
    }
}