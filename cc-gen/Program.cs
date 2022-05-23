using System.CommandLine;
using Jitesoft.CcGen.Commands;

var rootCommand = new RootCommand("Change Log Generator");

rootCommand.AddCommand(new InitCommand());
rootCommand.AddCommand(new GenerateCommand());
await rootCommand.InvokeAsync(args);
