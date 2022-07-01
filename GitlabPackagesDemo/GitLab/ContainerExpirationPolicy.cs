using System;
using Newtonsoft.Json;

namespace GitlabPackagesDemo.GitLab;

public class ContainerExpirationPolicy
{
    [JsonProperty("cadence")]
    public string Cadence { get; set; }
    
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }
    
    [JsonProperty("keep_n")]
    public int? KeepN { get; set; }
    
    [JsonProperty("older_than")]
    public string OlderThan { get; set; }
    
    [JsonProperty("name_regex")]
    public string NameRegex { get; set; }
    
    [JsonProperty("name_regex_keep")]
    public object NameRegexKeep { get; set; }
    
    [JsonProperty("next_run_at")]
    public DateTime? NextRunAt { get; set; }
}