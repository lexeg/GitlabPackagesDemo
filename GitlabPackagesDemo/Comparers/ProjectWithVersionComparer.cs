using System;
using System.Collections.Generic;

namespace GitlabPackagesDemo.Comparers;

public class ProjectWithVersionComparer : IEqualityComparer<(string Project, string Version)>
{
    public bool Equals((string Project, string Version) x, (string Project, string Version) y)
    {
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

    public int GetHashCode((string Project, string Version) obj)
    {
        var hCode = obj.Project.GetHashCode(); // ^ obj.Version.GetHashCode();
        return hCode.GetHashCode();
    }
}