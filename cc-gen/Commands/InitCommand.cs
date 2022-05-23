using System.CommandLine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Jitesoft.CcGen.Commands;

public class InitCommand : Command
{

    public InitCommand() 
        : base("init", "Initializes local (or global) configuration file")
    {
        var globalOption = new Option<bool>(
            name: "--global",
            description:
            "If configuration to initialize should be global (will be created in user home folder)",
            getDefaultValue: () => false
        );
        
        AddOption(globalOption);
        
        this.SetHandler(async (bool global) => await InitConfig(global), globalOption);
    }
    
    private async Task InitConfig(bool global)
    {
        if (global)
        {
            await CreateGlobalConfig();
            return;
        }
        
        await CreateLocalConfig();
    }

    private async Task CreateGlobalConfig()
    {
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.cc-gen";

        if (File.Exists(homePath))
        {
            Console.WriteLine($"Global configuration file already exist. To re-init, remove file at {homePath}");
        }
        else
        {
            var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            await File.WriteAllTextAsync(homePath, serializer.Serialize(new Config()));
        }
    }

    private async Task CreateLocalConfig()
    {
        var localPath = Environment.CurrentDirectory + "/.cc-gen";
        if (File.Exists(localPath))
        {
            Console.WriteLine($"Local configuration file already exist. To re-init, remove file at {localPath}");
        }
        else
        {
            var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            await File.WriteAllTextAsync(localPath, serializer.Serialize(new Config()));
        }
    }
}
