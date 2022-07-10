using GitlabPackagesDemo.Settings;
using GitlabPackagesDemo.Views;
using Microsoft.Extensions.Options;

namespace GitlabPackagesDemo.Helpers;

public class DialogFactory
{
    public SettingsDialog CreateSettingsDialog(IOptions<GitLabSettings> settings) => new(settings);

    public SearchDialog CreateSearchDialog(IOptions<SearchSettings> settings) => new(settings);

    public SavingPackagesDialog CreateSavingPackagesDialog() => new();
}