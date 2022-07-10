using GitlabPackagesDemo.ViewModels;

namespace GitlabPackagesDemo.Views;

public partial class SavingPackagesDialog
{
    public SavingPackagesDialog()
    {
        InitializeComponent();
        DataContext = new SavingPackagesViewModel(this);
    }
}