﻿using LibGit2Sharp;

namespace Jitesoft.Libs.ConventionalCommits;

public static class RepositoryExtension
{
    public static IList<Conventional> GetConventionalCommits(this Repository self, Tag? from = null, Tag? until = null, CommitSortStrategies strategy = CommitSortStrategies.Topological)
    {
        var fromSha = from?.Target?.Sha ?? self.Head.Tip.Sha;
        var toSha = until?.Target?.Sha;

        var commits = self.Commits.QueryBy(new CommitFilter
        {
            SortBy = strategy,
            IncludeReachableFrom = fromSha,
        });
        
        var filtered = commits.Where(c => c.IsConventional());
        var result = new List<Conventional>();
        foreach (var c in filtered)
        {
            if (toSha != null && c.Sha == toSha)
            {
                break;
            }
            
            if (c.ParseConventional(out var conventional))
            {
                result.Add(conventional!);
            }
        }

        return result;
    }
}
