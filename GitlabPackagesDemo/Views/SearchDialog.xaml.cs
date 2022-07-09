using GitlabPackagesDemo.Settings;
using GitlabPackagesDemo.ViewModels;
using Microsoft.Extensions.Options;

namespace GitlabPackagesDemo.Views;

public partial class SearchDialog
{
    public SearchDialog(IOptions<SearchSettings> settings)
    {
        InitializeComponent();
        DataContext = new SearchViewModel(this, settings);
    }
}