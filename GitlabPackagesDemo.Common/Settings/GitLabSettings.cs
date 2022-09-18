namespace GitlabPackagesDemo.Common.Settings;

public class GitLabSettings
{
    public static string Key => nameof(GitLabSettings);
    public string Host { get; set; }
    public string PrivateToken { get; set; }
}