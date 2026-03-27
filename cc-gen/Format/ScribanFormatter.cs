using Scriban;
using Scriban.Runtime;

namespace Jitesoft.CcGen.Format;

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
                ["committer_email"] = commit.Commit.Committer.Email,
                ["author_name"] = commit.Commit.Author.Name,
                ["author_when"] =  commit.Commit.Author.When,
                ["author_email"] = commit.Commit.Author.Email,
                ["message"] = commit.Commit.Message,
                ["breaking"] = commit.Breaking,
                ["type"] = commit.Type,
                ["subtype"] = commit.SubType,
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
            ["type"] = type,
            ["subtype"] = ""
        });
        return template.Render(context);
    }
    #endregion
}
