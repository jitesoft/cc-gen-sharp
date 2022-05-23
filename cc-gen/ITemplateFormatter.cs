using Jitesoft.Libs.ConventionalCommits;

namespace Jitesoft.CcGen;

public interface ITemplateFormatter
{
    public string FormatCommits(IEnumerable<IGrouping<string ,Conventional>> commits);
}
