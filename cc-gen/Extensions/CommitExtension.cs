using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace Jitesoft.CcGen.Extensions;

/// <summary>
/// Extension of the <see cref="Commit"/> class from LibGit2Sharp to
/// parse and test commits for conventional commits.
/// </summary>
public static class CommitExtension
{
    private static readonly Regex ConventionalParseRegex = new(@"^(?<type>\w+)([(](?<subtype>.+)[)])?(?<break>[!]?)[:]\s(?<header>.*)([\r\n|\r|\n]{1,2}(?s)(?<body>.*))?");

    /// <summary>
    /// Test to see if a specific commit is a conventional commit.
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsConventional(this Commit self)
    {
        return ConventionalParseRegex.IsMatch(self.Message);
    }
    
    /// <summary>
    /// Parse a commit into a conventional commit.
    /// </summary>
    /// <param name="self">The commit, self.</param>
    /// <returns>Parsed conventional commit.</returns>
    /// <exception cref="Exception">On invalid commit.</exception>
    public static Conventional ParseConventional(this Commit self)
    {
        if (!self.IsConventional())
        {
            throw new Exception("Commit is not a conventional commit.");
        }
        
        var parsed = ConventionalParseRegex.Matches(self.Message);
        return new Conventional
        {
            Type = parsed[0].Groups["type"].Value.Trim(),
            SubType = parsed[0].Groups["subtype"].Value.Trim(),
            Header = parsed[0].Groups["header"].Value.Trim(),
            Body = parsed[0].Groups["body"].Value.Trim(),
            Breaking = !string.IsNullOrWhiteSpace(parsed[0].Groups["break"].Value.Trim()) || 
                       parsed[0].Groups["body"].Value.Contains("BREAKING CHANGE"),
            Commit = self
        };
    }
    
    /// <summary>
    /// Try parse a commit into a conventional commit.
    /// If the commit passed is not actually a conventional commit
    /// the conventional out parameter will be null and the
    /// function will return false.
    /// </summary>
    /// <param name="self">The commit, self.</param>
    /// <param name="conventional">Conventional commit which will be set on success.</param>
    /// <returns>True on success, else false.</returns>
    public static bool TryParseConventional(this Commit self, out Conventional? conventional)
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
                Breaking = !string.IsNullOrWhiteSpace(parsed[0].Groups["break"].Value.Trim()) || 
                           parsed[0].Groups["body"].Value.Contains("BREAKING CHANGE"),
                Commit = self
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
