﻿using System.CommandLine;
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

    public GenerateCommand(IFormatter formatter)
        : base("generate", "Generate changelog")
    {
        _formatter = formatter;
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

        AddOption(fromOpt);
        AddOption(toOpt);
        AddOption(latestOpt);

        AddAlias("gen");

        this.SetHandler(
            (string? from, string? to, bool latest) => Generate(from, to, latest), 
            fromOpt, 
            toOpt,
            latestOpt
        );
    }

    private string Generate(string? from, string? to, bool latest)
    {
        var repository = LoadLocalRepository();

        string? fromSha = null;
        string? toSha = null;
        
        if (latest)
        {
            fromSha = repository.Head.Tip.Sha;
            toSha = repository.GetLatestTag();
        }
        else
        {
            if (from != null)
            {
                fromSha = repository.Tags[from].Target.Sha;
            }

            if (to != null)
            {
                toSha = repository.Tags[to].Target.Sha;
            }
        }

        var commits = repository.GetConventionalCommits(
            fromSha: fromSha,
            toSha: toSha
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

        if (!string.IsNullOrWhiteSpace(config.Footer))
        {
            str += "  " + Environment.NewLine + config.Footer;
        }
        Console.Write(str);
        return str;
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
