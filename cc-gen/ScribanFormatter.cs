using Jitesoft.Libs.ConventionalCommits;
using LibGit2Sharp;
using Scriban;

namespace Jitesoft.CcGen;

public class ScribanFormatter : ITemplateFormatter
{
    private readonly Config _config;

    public ScribanFormatter(Config config)
    {
        _config = config;
    }

    private string FormatCommit(Conventional commit)
    {
        var tmp = Template.Parse(_config.Commit.Trim());
        return tmp.Render(commit);
    }

    private string FormatBreakingCommit(Conventional commit)
    {
        var tmp = Template.Parse(_config.BreakingCommit.Trim());
        return tmp.Render(commit);
    }

    public string FormatType(string type)
    {
        var tmp = Template.Parse(_config.Type.Trim());
        return tmp.Render(new { type });
    }

    public string FormatCommits(IEnumerable<IGrouping<string, Conventional>> commits)
    {
        var stringWriter = new StringWriter();
        stringWriter.WriteLine(_config.Header);
        stringWriter.WriteLine();

        foreach (var val in commits)
        {
            stringWriter.WriteLine(FormatType(val.Key));
            foreach (var commit in val)
            {
                if (commit.Breaking && _config.GroupBreakingChanges)
                {
                    continue;
                }

                stringWriter.WriteLine(
                    commit.Breaking ? FormatBreakingCommit(commit) : FormatCommit(commit)
                );
            }

            stringWriter.WriteLine();
        }

        if (_config.GroupBreakingChanges && commits.Any(l => l.Any(c => c.Breaking)))
        {
            stringWriter.WriteLine(_config.GroupedBreakingHeader);
            stringWriter.WriteLine();

            foreach (var val in commits)
            {
                var breaking = val.Where(c => c.Breaking).ToList();
                if (breaking.Count == 0)
                {
                    continue;
                }

                stringWriter.WriteLine(FormatType(val.Key));
                foreach (var commit in breaking)
                {
                    stringWriter.WriteLine(
                        FormatCommit(commit)
                    );
                }

                stringWriter.WriteLine();
            }
        }


        return stringWriter.ToString();
    }
}
