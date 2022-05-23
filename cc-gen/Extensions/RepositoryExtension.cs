using LibGit2Sharp;

namespace Jitesoft.CcGen.Extensions;

/// <summary>
/// Extension of the <see cref="Repository"/> class from LibGit2Sharp to
/// fetch and convert conventional commits.
/// </summary>
public static class RepositoryExtension
{
    /// <summary>
    /// Get all conventional commits.
    /// </summary>
    /// <param name="self">Self.</param>
    /// <param name="fromSha">Sha to start from.</param>
    /// <param name="toSha">Sha to stop before.</param>
    /// <param name="strategy">Get log strategy.</param>
    /// <returns>List of commits which are conventional commits.</returns>
    public static IEnumerable<Commit> GetConventionalCommits(this Repository self, string? fromSha = null, string? toSha = null, CommitSortStrategies strategy = CommitSortStrategies.Topological)
    {
        fromSha ??= self.Head.Tip.Sha;
        
        var commits = self.Commits.QueryBy(new CommitFilter
        {
            SortBy = strategy,
            IncludeReachableFrom = fromSha
        });

        foreach (var c in commits)
        {
            if (toSha != null && c.Sha == toSha)
            {
                yield break;
            }
            
            if (c.IsConventional())
            {
                yield return c;
            }
        }
    }

}
