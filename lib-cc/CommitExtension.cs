using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace Jitesoft.Libs.ConventionalCommits;

public static class CommitExtension
{
    private static readonly Regex ConventionalParseRegex = new Regex(@"^(?<type>\w+)([(](?<subtype>.+)[)])?[:]\s(?<header>.*)([\n]{2}(?s)(?<body>.*))?");

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
                Type = parsed[0].Groups["type"].Value.Trim(),
                SubType = parsed[0].Groups["subtype"].Value.Trim(),
                Header = parsed[0].Groups["header"].Value.Trim(),
                Body = parsed[0].Groups["body"].Value.Trim(),
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
