using Newtonsoft.Json;

namespace GitlabPackagesDemo.Common.GitLab;

public class Namespace
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("path")]
    public string Path { get; set; }
    
    [JsonProperty("kind")]
    public string Kind { get; set; }
    
    [JsonProperty("full_path")]
    public string FullPath { get; set; }
    
    [JsonProperty("parent_id")]
    public int? ParentId { get; set; }
    
    [JsonProperty("avatar_url")]
    public object AvatarUrl { get; set; }
    
    [JsonProperty("web_url")]
    public string WebUrl { get; set; }
}