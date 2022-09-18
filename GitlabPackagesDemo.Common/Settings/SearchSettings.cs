namespace GitlabPackagesDemo.Common.Settings;

public class SearchSettings
{
    public static string Key => nameof(SearchSettings);
    public string SearchText { get; set; }
    public string FileExtension { get; set; }
}