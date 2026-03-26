using LibGit2Sharp;

namespace Jitesoft.CcGen.Extensions;

/// <summary>
/// Extension of the <see cref="Repository"/> class from LibGit2Sharp to
/// fetch and convert conventional commits.
/// </summary>
public static class RepositoryExtension
{
    /// <param name="self">Self.</param>
    extension(IRepository self)
    {
        /// <summary>
        /// Get all conventional commits.
        /// </summary>
        /// <param name="fromSha">Sha to start from.</param>
        /// <param name="toSha">Sha to stop before.</param>
        /// <param name="strategy">Get log strategy.</param>
        /// <returns>List of commits which are conventional commits.</returns>
        public IEnumerable<Commit> GetConventionalCommits(string? fromSha = null, string? toSha = null, CommitSortStrategies strategy = CommitSortStrategies.Topological)
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

        /// <summary>
        /// Get commit SHA of latest tag from the repository head.
        /// If the tag is the same SHA as the latest commit, second tag will be returned.
        /// </summary>
        /// <returns>SHA of the tagged commit.</returns>
        public string? GetLatestTag()
        {
            var fromSha = self.Head.Tip.Sha;
            string? toSha = null;

            // We want to read from other way around (latest first)!
            var allTags = self.Tags.OrderBy(x => ((Commit)x.PeeledTarget).Committer.When).Reverse().ToList();

            if (allTags.Count == 0)
            {
                return toSha;
            }

            if (allTags.First().Target.Sha != fromSha)
            {
                toSha = allTags.First().Target.Sha;
            }
            else if (allTags.Count > 1)
            {
                toSha = allTags[1].Target.Sha;
            }

            return toSha;
        }
    }
}
