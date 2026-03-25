using System.CommandLine;
using Jitesoft.CcGen;
using Jitesoft.CcGen.Commands;
using Jitesoft.CcGen.Format;

var rootCommand = new RootCommand("Change Log Generator");

var config = Config.LoadConfiguration();
var scribanFormatter = new ScribanFormatter(config);

rootCommand.Add(new InitCommand());
rootCommand.Add(
    new GenerateCommand(
        new GenericFormatter(scribanFormatter, scribanFormatter, config),
        config
    )
);

await rootCommand.Parse(args).InvokeAsync();
