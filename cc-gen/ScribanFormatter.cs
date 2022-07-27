using Jitesoft.CcGen.Format;
using Scriban;

namespace Jitesoft.CcGen;

/// <summary>
/// Class which uses Scriban template engine to parse
/// templates into strings.
/// </summary>
public class ScribanFormatter : ICommitFormatter, ITypeFormatter
{
    private readonly Config _config;

    public ScribanFormatter(Config config)
    {
        _config = config;
    }

    #region ICommitFormatter members.
    public string FormatCommit(Conventional commit, bool isBreaking = false)
    {
        var tmp = Template.Parse(isBreaking ? _config.BreakingCommit.Trim() : _config.Commit.Trim());
        return tmp.Render(commit);
    }
    #endregion

    #region ITypeFormatter members.
    public string FormatType(string type)
    {
        var tmp = Template.Parse(_config.Type.Trim());
        return tmp.Render(new { type });
    }
    #endregion
}
