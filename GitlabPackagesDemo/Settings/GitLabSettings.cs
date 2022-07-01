namespace GitlabPackagesDemo.Settings;

public class GitLabSettings
{
    public static string Key => nameof(GitLabSettings);
    public string Host { get; set; }
    public string PrivateToken { get; set; }
}