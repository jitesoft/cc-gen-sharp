using Scriban;

namespace Jitesoft.CcGen;

/// <summary>
/// Class which uses Scriban template engine to parse
/// templates into strings.
/// </summary>
public class ScribanFormatter
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

    private string FormatBreakingChanges(IEnumerable<IGrouping<string, Conventional>> commits)
    {
        var stringWriter = new StringWriter();
        commits = commits.ToList();

        if (commits.Any(l => l.Any(c => c.Breaking)))
        {
            stringWriter.WriteLine(_config.GroupedBreakingHeader);
            stringWriter.WriteLine();

            foreach (var val in commits)
            {
                var commitsAsString = FormatCommitList(
                    val.Where(c => c.Breaking).ToList()
                );
                
                
                if (commitsAsString.Trim().Length > 0)
                {
                    stringWriter.WriteLine(FormatType(val.Key));
                    stringWriter.Write(commitsAsString);
                    stringWriter.WriteLine();
                }
            }
        }

        return stringWriter.ToString();
    }

    private string FormatCommitList(IList<Conventional> commits)
    {
        var stringWriter = new StringWriter();
        
        foreach (var commit in commits)
        {
            stringWriter.WriteLine(
                commit.Breaking && !_config.GroupBreakingChanges ? FormatBreakingCommit(commit) : FormatCommit(commit)
            );
        }

        return stringWriter.ToString();
    }
    
    public string FormatCommits(IEnumerable<IGrouping<string, Conventional>> commits)
    {
        commits = commits.ToList();
        var stringWriter = new StringWriter();
        stringWriter.WriteLine(_config.Header);
        stringWriter.WriteLine();

        foreach (var val in commits)
        {
            var commitsAsString = FormatCommitList(
                _config.GroupBreakingChanges ? 
                    val.Where(c => !c.Breaking).ToList() : 
                    val.ToList()
            );

            if (commitsAsString.Trim().Length > 0)
            {
                stringWriter.WriteLine(FormatType(val.Key));
                stringWriter.Write(commitsAsString);
                stringWriter.WriteLine();
            }
            
        }

        if (_config.GroupBreakingChanges)
        {
            stringWriter.Write(
                FormatBreakingChanges(commits)
            );
        }


        return stringWriter.ToString();
    }
}
