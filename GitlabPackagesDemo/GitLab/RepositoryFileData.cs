using Newtonsoft.Json;

namespace GitlabPackagesDemo.GitLab;

public class RepositoryFileData
{
    [JsonProperty("basename")]
    public string BaseName { get; set; }
    
    [JsonProperty("data")]
    public string Data { get; set; }
    
    [JsonProperty("path")]
    public string Path { get; set; }
    
    [JsonProperty("filename")]
    public string FileName { get; set; }
    
    [JsonProperty("id")]
    public object Id { get; set; }
    
    [JsonProperty("ref")]
    public string Ref { get; set; }
    
    [JsonProperty("startline")]
    public int StartLine { get; set; }
    
    [JsonProperty("project_id")]
    public int ProjectId { get; set; }
}