using GitlabPackagesDemo.GitLab;

namespace GitlabPackagesDemo.Common;

public class RepoFiles
{
    public GitRepository Repository { get; set; }

    public RepositoryFileData[] Files { get; set; }
}