using System;
using System.Collections.Generic;
using GitlabPackagesDemo.Common.Data;

namespace GitlabPackagesDemo.Comparers;

public class ProjectDataComparer : IEqualityComparer<ProjectData>
{
    public bool Equals(ProjectData x, ProjectData y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        if (x.Version == null && y.Version == null)
        {
            return x.Project.Equals(y.Project, StringComparison.OrdinalIgnoreCase);
        }

        if (x.Version == null || y.Version == null)
        {
            return false;
        }

        return x.Project.Equals(y.Project, StringComparison.OrdinalIgnoreCase) &&
               x.Version.Equals(y.Version, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(ProjectData obj)
    {
        var project = obj.Project ?? string.Empty;
        var version = obj.Version ?? string.Empty;
        return project.GetHashCode() ^ version.GetHashCode();
    }
}