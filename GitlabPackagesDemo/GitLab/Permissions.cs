using Newtonsoft.Json;

namespace GitlabPackagesDemo.GitLab;

public class Permissions
{
    [JsonProperty("project_access")]
    public ProjectAccess ProjectAccess { get; set; }
    
    [JsonProperty("group_access")]
    public GroupAccess GroupAccess { get; set; }
}