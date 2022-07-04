using System;
using System.Collections.Generic;
using GitlabPackagesDemo.Common;

namespace GitlabPackagesDemo.Comparers;

public class ProjectWithVersionComparer : IEqualityComparer<ProjectData>
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
        var hCode = obj.Project.GetHashCode(); // ^ obj.Version.GetHashCode();
        return hCode.GetHashCode();
    }
}