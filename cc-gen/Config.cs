using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Jitesoft.CcGen;

/// <summary>
/// Base configuration.
/// </summary>
public class Config
{
    public string Header { get; set; } = "# Change Log";
    public string Footer { get; set; } = "";

    public bool GroupBreakingChanges { get; set; } = true;
    public string GroupedBreakingHeader { get; set; } = "## Breaking changes";

    public string Commit { get; set; } =
        "  * [ {{ commit.sha | string.slice1 0 6 }} ] {{ header }} ({{ commit.committer.name }}) {{ commit.committer.when }}  ";

    public string BreakingCommit { get; set; } =
        "  * [ {{ commit.sha | string.slice1 0 6 }} ] **breaking** {{ header }} ({{ commit.committer.name }}) {{ commit.committer.when }}  ";
    
    public string Type { get; set; } = "### {{ type }}  ";

    public string DefaultType = "Misc";

    public Dictionary<string, List<string>> TypeMap { get; set; } = new()
    {
        { "Features", new() { "feature", "feat", } },
        { "Fixes", new() { "hotfix", "fix", } },
        { "Build", new() { "build", } },
        { "Chores", new() { "chore", } },
        { "Ci", new() { "ci", } },
        { "Documentation", new() { "docs", "documentation", } },
        { "Style", new() { "style", } },
        { "Refactoring", new() { "refactor", } },
        { "Pref", new() { "pref", } },
        { "Test", new() { "test", } },
    };

    public static Config LoadConfiguration()
    {
        Config config = new Config();
        var yamlDeserializer =
            new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();

        var localPath = Environment.CurrentDirectory + "/.cc-gen";

        if (!File.Exists(localPath))
        {
            var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.cc-gen";
            if (File.Exists(homePath))
            {
                config = yamlDeserializer.Deserialize<Config>(File.ReadAllText(homePath));
            }
        }
        else
        {
            config = yamlDeserializer.Deserialize<Config>(File.ReadAllText(localPath));
        }

        return config;
    }
}
