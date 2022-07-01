using Newtonsoft.Json;

namespace GitlabPackagesDemo.GitLab;

public class GroupAccess
{
    [JsonProperty("access_level")]
    public int AccessLevel { get; set; }
    
    [JsonProperty("notification_level")]
    public int NotificationLevel { get; set; }
}