using Jitesoft.CcGen.Format;
using Scriban;
using Scriban.Runtime;

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
        var template = Template.Parse(isBreaking ? _config.BreakingCommit.Trim() : _config.Commit.Trim());
        var context = new TemplateContext();
        context.PushGlobal(
            new ScriptObject()
            {
                ["sha"] = commit.Commit.Sha,
                ["header"] = commit.Header,
                ["committer_name"] = commit.Commit.Committer.Name,
                ["committer_when"] = commit.Commit.Committer.When,
            }
        );
        return template.Render(context);
    }
    #endregion

    #region ITypeFormatter members.
    public string FormatType(string type)
    {
        var template = Template.Parse(_config.Type.Trim());
        var context = new TemplateContext();
        context.PushGlobal(new ScriptObject()
        {
            ["type"] = type
        });
        return template.Render(context);
    }
    #endregion
}
