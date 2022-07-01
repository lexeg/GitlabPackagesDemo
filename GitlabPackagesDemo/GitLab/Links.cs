using Newtonsoft.Json;

namespace GitlabPackagesDemo.GitLab;

public class Links
{
    [JsonProperty("self")]
    public string Self { get; set; }
    
    [JsonProperty("issues")]
    public string Issues { get; set; }
    
    [JsonProperty("merge_requests")]
    public string MergeRequests { get; set; }
    
    [JsonProperty("repo_branches")]
    public string RepoBranches { get; set; }
    
    [JsonProperty("labels")]
    public string Labels { get; set; }
    
    [JsonProperty("events")]
    public string Events { get; set; }
    
    [JsonProperty("members")]
    public string Members { get; set; }
}