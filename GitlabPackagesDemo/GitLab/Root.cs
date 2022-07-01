using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitlabPackagesDemo.GitLab;

public class Root
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("name_with_namespace")]
    public string NameWithNamespace { get; set; }
    
    [JsonProperty("path")]
    public string Path { get; set; }
    
    [JsonProperty("path_with_namespace")]
    public string PathWithNamespace { get; set; }
    
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("default_branch")]
    public string DefaultBranch { get; set; }
    
    [JsonProperty("tag_list")]
    public List<object> TagList { get; set; }
    
    [JsonProperty("ssh_url_to_repo")]
    public string SshUrlToRepo { get; set; }
    
    [JsonProperty("http_url_to_repo")]
    public string HttpUrlToRepo { get; set; }
    
    [JsonProperty("web_url")]
    public string WebUrl { get; set; }
    
    [JsonProperty("readme_url")]
    public string ReadmeUrl { get; set; }
    
    [JsonProperty("avatar_url")]
    public object AvatarUrl { get; set; }
    
    [JsonProperty("forks_count")]
    public int ForksCount { get; set; }
    
    [JsonProperty("star_count")]
    public int StarCount { get; set; }
    
    [JsonProperty("last_activity_at")]
    public DateTime LastActivityAt { get; set; }
    
    [JsonProperty("namespace")]
    public Namespace Namespace { get; set; }
    
    [JsonProperty("_links")]
    public Links Links { get; set; }
    
    [JsonProperty("packages_enabled")]
    public bool? PackagesEnabled { get; set; }
    
    [JsonProperty("empty_repo")]
    public bool EmptyRepo { get; set; }
    
    [JsonProperty("archived")]
    public bool Archived { get; set; }
    
    [JsonProperty("visibility")]
    public string Visibility { get; set; }
    
    [JsonProperty("resolve_outdated_diff_discussions")]
    public bool ResolveOutdatedDiffDiscussions { get; set; }
    
    [JsonProperty("container_registry_enabled")]
    public bool ContainerRegistryEnabled { get; set; }
    
    [JsonProperty("container_expiration_policy")]
    public ContainerExpirationPolicy ContainerExpirationPolicy { get; set; }
    
    [JsonProperty("issues_enabled")]
    public bool IssuesEnabled { get; set; }
    
    [JsonProperty("merge_requests_enabled")]
    public bool MergeRequestsEnabled { get; set; }
    
    [JsonProperty("wiki_enabled")]
    public bool WikiEnabled { get; set; }
    
    [JsonProperty("jobs_enabled")]
    public bool JobsEnabled { get; set; }
    
    [JsonProperty("snippets_enabled")]
    public bool SnippetsEnabled { get; set; }
    
    [JsonProperty("service_desk_enabled")]
    public bool ServiceDeskEnabled { get; set; }
    
    [JsonProperty("service_desk_address")]
    public object ServiceDeskAddress { get; set; }
    
    [JsonProperty("can_create_merge_request_in")]
    public bool CanCreateMergeRequestIn { get; set; }
    
    [JsonProperty("issues_access_level")]
    public string IssuesAccessLevel { get; set; }
    
    [JsonProperty("repository_access_level")]
    public string RepositoryAccessLevel { get; set; }
    
    [JsonProperty("merge_requests_access_level")]
    public string MergeRequestsAccessLevel { get; set; }
    
    [JsonProperty("forking_access_level")]
    public string ForkingAccessLevel { get; set; }
    
    [JsonProperty("wiki_access_level")]
    public string WikiAccessLevel { get; set; }
    
    [JsonProperty("builds_access_level")]
    public string BuildsAccessLevel { get; set; }
    
    [JsonProperty("snippets_access_level")]
    public string SnippetsAccessLevel { get; set; }
    
    [JsonProperty("pages_access_level")]
    public string PagesAccessLevel { get; set; }
    
    [JsonProperty("operations_access_level")]
    public string OperationsAccessLevel { get; set; }
    
    [JsonProperty("analytics_access_level")]
    public string AnalyticsAccessLevel { get; set; }
    
    [JsonProperty("emails_disabled")]
    public bool? EmailsDisabled { get; set; }
    
    [JsonProperty("shared_runners_enabled")]
    public bool SharedRunnersEnabled { get; set; }
    
    [JsonProperty("lfs_enabled")]
    public bool LfsEnabled { get; set; }
    
    [JsonProperty("creator_id")]
    public int CreatorId { get; set; }
    
    [JsonProperty("import_status")]
    public string ImportStatus { get; set; }
    
    [JsonProperty("open_issues_count")]
    public int OpenIssuesCount { get; set; }
    
    [JsonProperty("ci_default_git_depth")]
    public int CiDefaultGitDepth { get; set; }
    
    [JsonProperty("ci_forward_deployment_enabled")]
    public bool? CiForwardDeploymentEnabled { get; set; }
    
    [JsonProperty("public_jobs")]
    public bool PublicJobs { get; set; }
    
    [JsonProperty("build_timeout")]
    public int BuildTimeout { get; set; }
    
    [JsonProperty("auto_cancel_pending_pipelines")]
    public string AutoCancelPendingPipelines { get; set; }
    
    [JsonProperty("build_coverage_regex")]
    public string BuildCoverageRegex { get; set; }
    
    [JsonProperty("ci_config_path")]
    public string CiConfigPath { get; set; }
    
    [JsonProperty("shared_with_groups")]
    public List<object> SharedWithGroups { get; set; }
    
    [JsonProperty("only_allow_merge_if_pipeline_succeeds")]
    public bool OnlyAllowMergeIfPipelineSucceeds { get; set; }
    
    [JsonProperty("allow_merge_on_skipped_pipeline")]
    public bool? AllowMergeOnSkippedPipeline { get; set; }
    
    [JsonProperty("restrict_user_defined_variables")]
    public bool RestrictUserDefinedVariables { get; set; }
    
    [JsonProperty("request_access_enabled")]
    public bool RequestAccessEnabled { get; set; }
    
    [JsonProperty("only_allow_merge_if_all_discussions_are_resolved")]
    public bool OnlyAllowMergeIfAllDiscussionsAreResolved { get; set; }
    
    [JsonProperty("remove_source_branch_after_merge")]
    public bool? RemoveSourceBranchAfterMerge { get; set; }
    
    [JsonProperty("printing_merge_request_link_enabled")]
    public bool PrintingMergeRequestLinkEnabled { get; set; }
    
    [JsonProperty("merge_method")]
    public string MergeMethod { get; set; }
    
    [JsonProperty("suggestion_commit_message")]
    public string SuggestionCommitMessage { get; set; }
    
    [JsonProperty("auto_devops_enabled")]
    public bool AutoDevopsEnabled { get; set; }
    
    [JsonProperty("auto_devops_deploy_strategy")]
    public string AutoDevopsDeployStrategy { get; set; }
    
    [JsonProperty("autoclose_referenced_issues")]
    public bool AutocloseReferencedIssues { get; set; }
    
    [JsonProperty("permissions")]
    public Permissions Permissions { get; set; }
}