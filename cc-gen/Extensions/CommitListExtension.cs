using LibGit2Sharp;

namespace Jitesoft.CcGen.Extensions;

/// <summary>
/// Extension class for commits in IEnumerable to allow
/// easy conversion of lists of commits.
/// </summary>
public static class CommitListExtension
{
    /// <param name="self">Self.</param>
    extension(IEnumerable<Commit> self)
    {
        /// <summary>
        /// Convert a list of commits to conventional commits.
        /// Will exclude any commit which is not a conventional commit.
        /// </summary>
        /// <returns>IEnumerable with conventional commits.</returns>
        public IEnumerable<Conventional> ConvertConventional()
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
}
