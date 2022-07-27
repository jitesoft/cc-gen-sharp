# CC-GEN

A generator of changelog from conventional commits.

## Work in progress

This program is a work in progress, some features works basically as intended, 
but more should be added and the code is quite messy.
Use at your own risk!

## Why?

There are a lot of changelog generators out there, the reason for this one is 
that the ones already existing either did not work with lightweight tags
or required a lot of extra things installed to work.  
The idea of cc-gen is to have a simple single binary which is easy to
run and easy to modify both in form of code and the templates/configuration
which is used to generate output.

## How?

There are currently two commands, `init` and `generate|gen`.  
The former creates a configuration file locally or globally which allows
you to change the templates used to generate the changelogs.  
The later will generate a changelog.

### Init

The init command can be invoked with a --global flag, in which case it will
create a .cc-gen file in your home directory with the default configuration.  

Without the --global flag, the file will be created in the directory where
the command was invoked.

The following configuration values are available:

`Header` - The header of the changelog, defaults to '# Change Log'  
`Footer` - The footer of the changelog, defaults to ''  
`GroupBreakingChanges` - If any breaking changes should have their own section
in the changelog, defaults to true.  
`GroupBreakingHeader` - Header for breaking changes, defautls to '## Breaking changes'  
`Type` - Template for 'Types'  
`DefaultType` - Type any non-matched commits will go under (commits following cc but not found in TypeMap)  
`TypeMap` - A map of 'types' and their possible values in the commit messages  
`BreakingCommit` - Template for breaking changes, only used if 'GroupBreakingChanges' is false  
`Commit` - Template for commits  

```text 
Default for 'Commit':
  * [ {{ commit.sha | string.slice1 0 6 }} ] {{ header }} ({{ commit.committer.name }}) {{ commit.committer.when }}  

Default for 'BreakingCommit' 
  * [ {{ commit.sha | string.slice1 0 6 }} ] **breaking** {{ header }} ({{ commit.committer.name }}) {{ commit.committer.when }}  
```

_Will later fill this part with a bit more information on what is passed to the template engine,
for now, check the 'Conventional.cs' class_

### gen | generate

The generate command will generate a full changelog by default. It will order each commit under a tag and its types.  
The following options can be passed: `--latest`, `--from <tag>` `--to <tag>`

If `--latest` is used, cc-gen will print the changes between current tip and the last tag.  
If `--from` is used, a changelog without tags (will change in later version) will be 
printed from the given tag til the `--to` tag or first commit.

## With what?

The application makes use of [LibGit2Sharp](https://github.com/libgit2/libgit2sharp/) to 
work with the git log, [Scriban](https://github.com/scriban/scriban) to handle the templates 
and [YamlDotNet](https://github.com/aaubry/YamlDotNet) for Yaml parsing.  
The command line helper used is [System.CommandLine](https://github.com/dotnet/command-line-api) 
and tests makes use of [XUnit](https://xunit.net/) and [Moq](https://github.com/moq/moq)

## Examples!

For an example, check the tags for cc-gen at [GitLab](https://gitlab.com/jitesoft/open-source/c-sharp/cc-gen/-/tags) 
or [GitHub](github.com/jitesoft/cc-gen-sharp/releases).
