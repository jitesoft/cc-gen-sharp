using LibGit2Sharp;

namespace Jitesoft.Libs.ConventionalCommits;

public static class RepositoryExtension
{
    public static IEnumerable<Commit> GetConventionalCommits(this Repository self, string? fromSha = null, string? toSha = null, CommitSortStrategies strategy = CommitSortStrategies.Topological)
    {
        fromSha ??= self.Head.Tip.Sha;
        
        var commits = self.Commits.QueryBy(new CommitFilter
        {
            SortBy = strategy,
            IncludeReachableFrom = fromSha,
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

    public static IEnumerable<Conventional> ConvertConventional(this IEnumerable<Commit> self)
    {
        foreach (var c in self)
        {
            if (c.ParseConventional(out var conventional))
            {
                yield return conventional!;
            }
        }
    }
}
