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
            DataContext = new RepositoriesViewModel(this, new FileSaver());
        }
    }
}