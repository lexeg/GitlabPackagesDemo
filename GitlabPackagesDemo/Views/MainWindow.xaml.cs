using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.ViewModels;

namespace GitlabPackagesDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            // TODO: DI-containers
            DataContext = new RepositoriesViewModel(this, new FileSaver(), new RepositoryService());
        }
    }
}