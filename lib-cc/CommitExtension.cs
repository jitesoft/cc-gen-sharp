using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace Jitesoft.Libs.ConventionalCommits;

public static class CommitExtension
{
    private static readonly Regex ConventionalParseRegex = new Regex("^(\\w+)[(]?(\\w+)?[)]?[:]\\s(.*)([\\n]+(?s)(.*))?");

    public static bool IsConventional(this Commit self)
    {
        return ConventionalParseRegex.IsMatch(self.Message);
    }
    
    public static bool ParseConventional(this Commit self, out Conventional? conventional)
    {
        if (!self.IsConventional())
        {
            conventional = null;
            return false;
        }

        try
        {
            var parsed = ConventionalParseRegex.Matches(self.Message);
            conventional = new Conventional
            {
                Type = parsed[0].Groups[1].Value.Trim(),
                SubType = parsed[0].Groups[2].Value.Trim(),
                Header = parsed[0].Groups[3].Value.Trim(),
                Body = parsed[0].Groups[4].Value.Trim(),
            };
        }
        catch
        {
            conventional = null;
            return false;
        }

        return true;
    }
}
