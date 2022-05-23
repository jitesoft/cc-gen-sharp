using System.CommandLine;
using Jitesoft.Libs.ConventionalCommits;
using LibGit2Sharp;

namespace Jitesoft.CcGen.Commands;

public class GenerateCommand : Command
{
    public GenerateCommand() 
        : base("generate", "Generate changelog")
    {
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

            // We want to read from other way around (latest first)!
            var allTags = repository.Tags.Reverse().ToList();

            if (allTags.Count != 0)
            {
                if (allTags.First().Target.Sha != fromSha)
                {
                    toSha = allTags.First().Target.Sha;
                } 
                else if (allTags.Count > 1)
                {
                    toSha = allTags[1].Target.Sha;
                }
            }
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
        
        var grouped = commits.GroupBy(x => ParseType(x.Type));
        
        
        
        var formatter = new ScribanFormatter(Config.LoadConfiguration());

        
        var str = formatter.FormatCommits(grouped);
        
        Console.Write(str);
        return str;
    }

    private static Repository LoadLocalRepository()
    {
    
        // Find root folder.
        var path = Environment.CurrentDirectory;
    
#if DEBUG
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
