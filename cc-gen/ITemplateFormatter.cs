using Jitesoft.Libs.ConventionalCommits;

namespace Jitesoft.CcGen;

public interface ITemplateFormatter
{
    public string FormatType(string type);
    
    public string FormatCommit(Conventional commit);

    public string FormatCommits(IEnumerable<IGrouping<string ,Conventional>> commits);
}
