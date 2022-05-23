using LibGit2Sharp;

namespace Jitesoft.CcGen.Extensions;

/// <summary>
/// Extension class for commits in IEnumerable to allow
/// easy conversion of lists of commits. 
/// </summary>
public static class CommitListExtension
{
    /// <summary>
    /// Convert a list of commits to conventional commits.
    /// Will exclude any commit which is not a conventional commit.
    /// </summary>
    /// <param name="self">Self.</param>
    /// <returns>IEnumerable with conventional commits.</returns>
    public static IEnumerable<Conventional> ConvertConventional(this IEnumerable<Commit> self)
    {
        foreach (var c in self)
        {
            if (c.TryParseConventional(out var conventional))
            {
                yield return conventional!;
            }
        }
    }
}
