using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Jitesoft.CcGen.Extensions;
using Jitesoft.CcGen.Format;
using LibGit2Sharp;

namespace Jitesoft.CcGen.Commands;

/// <summary>
/// Generate command.
///
/// This class takes care of the logic to generate the changelog.
/// </summary>
public class GenerateCommand : Command
{
    private readonly IFormatter _formatter;
    private readonly Config _config;
    private readonly IRepository _repository;

    public GenerateCommand(IFormatter formatter, Config config)
        : base("generate", "Generate changelog")
    {
        _formatter = formatter;
        _config = config;
        _repository = LoadLocalRepository();

        var fromOpt = new Option<string?>(
            name: "--from", 
            description: "Tag to use as start point, defaults to current head tip", 
            getDefaultValue: () => null
        );

        var toOpt = new Option<string?>(
            name: "--to",
            description: "Tag to use as start point, defaults first commit", 
            getDefaultValue: () => null
        );

        var latestOpt = new Option<bool>(
            name: "--latest",
            description: "Print commits between current commit and latest tag (or next tag if current commit is tagged)",
            getDefaultValue: () => false
        );

        var fullOpt = new Option<bool>(
            name: "--full",
            description: "Generates full changelog including all tags.",
            getDefaultValue: () => false
        );

        AddOption(fromOpt);
        AddOption(toOpt);
        AddOption(latestOpt);
        AddOption(fullOpt);

        AddAlias("gen");

        this.SetHandler(
            (string? from, string? to, bool latest, bool full) => Generate(from, to, latest, full),
            fromOpt, 
            toOpt,
            latestOpt,
            fullOpt
        );
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    private string GenerateFull()
    {
        var tags = _repository.Tags.OrderBy(
                x => ((Commit)x.PeeledTarget).Committer.When
            )
            .Reverse()
            .ToList();

        if (tags.Count <= 1)
        {
            // Generate all as a single log.
            return GenerateBetween(null, null);
        }

        var stringWriter = new StringWriter();

        // TODO: Cleanup.

        var tagName = "Unreleased";
        var prevSha = _repository.Head.Tip.Sha;
        var i = 0;

        if (tags[0].Target.Sha == prevSha)
        {
            tagName = tags[0].FriendlyName;
            i++;
        }

        for (; i < tags.Count; i++)
        {
            stringWriter.WriteLine($"## {tagName}");
            stringWriter.WriteLine("");

            string next = tags[i].Target.Sha;
            tagName = tags[i].FriendlyName;

            stringWriter.Write(GenerateBetweenSha(prevSha, next));
            prevSha = next;
        }

        // Last tag.
        stringWriter.WriteLine($"## {tagName}");
        stringWriter.WriteLine("");
        stringWriter.Write(GenerateBetweenSha(prevSha, null));

        return stringWriter.ToString();
    }

    private string GenerateLatest()
    {
        var fromSha = _repository.Head.Tip.Sha;
        var toSha = _repository.GetLatestTag();

        return GenerateBetweenSha(fromSha, toSha);
    }


    private string GenerateBetweenSha(string? from, string? to)
    {
        var commits = _repository.GetConventionalCommits(
            fromSha: from,
            toSha: to
        ).ConvertConventional().ToList();

        var config = Config.LoadConfiguration();

        string ParseType(string type)
        {
            // Make this better later, for now, meh.
            foreach (var types in config.TypeMap)
            {
                if (types.Value.Contains(type.ToLower()))
                {
                    return types.Key;
                }
            }

            return config.DefaultType;
        }

        var str = _formatter.FormatCommits(
            commits
                .GroupBy(x => ParseType(x.Type))
                .OrderBy(c => c.Key)
                .ToDictionary(
                    c => c.Key,
                    c => c.AsEnumerable()
                )
        );

        return str;
    }

    private string GenerateBetween(string? from, string? to)
    {
        if (from != null)
        {
            from = _repository.Tags[from].Target.Sha;
        }

        if (to != null)
        {
            to = _repository.Tags[to].Target.Sha;
        }

        return GenerateBetweenSha(from, to);
    }


    private void Generate(string? from, string? to, bool latest, bool full)
    {
        var result = "";
        if (full)
        {
            result = GenerateFull();
        } else if (latest)
        {
            result = GenerateLatest();
        }
        else
        {
            result = GenerateBetween(from, to);
        }



        if (!string.IsNullOrWhiteSpace(_config.Header))
        {
            Console.WriteLine(_config.Header);
            Console.WriteLine();
        }

        Console.Write(result);

        if (!string.IsNullOrWhiteSpace(_config.Footer))
        {
            Console.WriteLine("  " + Environment.NewLine + _config.Footer);
        }
    }

    private static Repository LoadLocalRepository()
    {
        var path = Environment.CurrentDirectory;

#if DEBUG
        // When running from IDE, the 'current directory' is a sub-directory.
        var found = false;
        while (!found)
        {
            path += "/..";
            if (Directory.Exists(path + "/.git"))
            {
                found = true;
            }
        }
#endif

        var repository = new Repository(path);
        return repository;
    }
}
