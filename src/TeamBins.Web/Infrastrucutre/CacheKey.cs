using System.Globalization;

namespace TeamBins.Infrastrucutre
{
    public class CacheKey
    {
        public static readonly string Projects = "Projects";
        public static readonly string Teams = "Teams";
        public static readonly string Issues = "Issues";
        public static readonly string Users = "Users";
        public static readonly string Statuses = "Statuses";
        public static readonly string Priorities = "Priorities";
        public static readonly string Categories = "Categories";

        public static string GetKey(string key,int teamId)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", key, teamId);
        }
    }
}