using System.CommandLine;
using Jitesoft.CcGen;
using Jitesoft.CcGen.Commands;
using Jitesoft.CcGen.Format;

var rootCommand = new RootCommand("Change Log Generator");

var config = Config.LoadConfiguration();
var scribanFormatter = new ScribanFormatter(config);

rootCommand.AddCommand(new InitCommand());
rootCommand.AddCommand(
    new GenerateCommand(
        new GenericFormatter(scribanFormatter, scribanFormatter, config)
    )
);

await rootCommand.InvokeAsync(args);
