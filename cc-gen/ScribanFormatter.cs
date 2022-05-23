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

    public string FormatCommit(Conventional commit)
    {
        var tmp = Template.Parse(_config.Commit.Trim());
        return tmp.Render(commit);
    }

    public string FormatType(string type)
    {
        var tmp = Template.Parse(_config.Type.Trim());
        return tmp.Render(new { type });
    }

    public string FormatCommits(IEnumerable<IGrouping<string ,Conventional>> commits)
    {
        var stringWriter = new StringWriter();
        
        foreach (var val in commits)
        {
            stringWriter.WriteLine(FormatType(val.Key));
            foreach (var commit in val)
            {
                stringWriter.WriteLine(FormatCommit(commit));
            }
            stringWriter.WriteLine();
        }
        
        return stringWriter.ToString();
    }
}
