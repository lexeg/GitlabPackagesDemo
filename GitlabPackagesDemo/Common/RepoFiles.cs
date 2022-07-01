using GitlabPackagesDemo.GitLab;

namespace GitlabPackagesDemo.Common;

public class RepoFiles
{
    public Root Repository { get; set; }

    public RootFile[] Files { get; set; }
}