using System;
using System.IO;
using System.Linq;
using Jitesoft.Libs.ConventionalCommits;
using LibGit2Sharp;
using Xunit;

namespace lib_cc.tests;

public class RepositoryExtensionTest
{
    private Repository LoadLocalRepository()
    {
        // Find root folder.
        var path = Environment.CurrentDirectory;
        var found = false;
        while (!found)
        {
            path += "/..";
            if (Directory.Exists(path + "/.git"))
            {
                found = true;
            }
        }
        
        var repository = new Repository(path);
        return repository;
    }

    [Fact]
    public void TestGetConventionalCommitsNoRange()
    {
        var commits = LoadLocalRepository().GetConventionalCommits()
            .ConvertConventional()
            .ToList();
        // Get last 4 to check.
        var toCheck = commits.TakeLast(4).ToArray();

        Assert.Equal("feat", toCheck[3].Type);
        Assert.Equal("solution", toCheck[3].SubType);    
        Assert.Equal("Added initial solution files.", toCheck[3].Header);
        Assert.Equal("Signed-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[3].Body);


        Assert.Equal("chore", toCheck[2].Type);
        Assert.Equal("", toCheck[2].SubType);    
        Assert.Equal("Dockerfile.", toCheck[2].Header);
        Assert.Equal("Signed-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[2].Body);

        
        Assert.Equal("feat", toCheck[1].Type);
        Assert.Equal("lib-cc", toCheck[1].SubType);    
        Assert.Equal("Created initial commit parser and test.", toCheck[1].Header);
        Assert.Equal("Including a body in this text to make sure it's around!\n\nSigned-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[1].Body);

        
        Assert.Equal("hotfix", toCheck[0].Type);
        Assert.Equal("lib-cc", toCheck[0].SubType);    
        Assert.Equal("Missing Conventional.cs file", toCheck[0].Header);
        Assert.Equal("Signed-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[0].Body);
    }
    
    [Fact]
    public void TestGetConventionalCommitsFirstTag()
    {
        var repository = LoadLocalRepository();
        var tagToStartFrom = repository.Tags["0.0.1"];
        
        
        var toCheck = LoadLocalRepository()
            .GetConventionalCommits(tagToStartFrom.Target.Sha)
            .ConvertConventional()
            .ToList();

        Assert.Equal(2, toCheck.Count);
        
        Assert.Equal("feat", toCheck[1].Type);
        Assert.Equal("solution", toCheck[1].SubType);    
        Assert.Equal("Added initial solution files.", toCheck[1].Header);
        Assert.Equal("Signed-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[1].Body);
        
        Assert.Equal("chore", toCheck[0].Type);
        Assert.Equal("", toCheck[0].SubType);    
        Assert.Equal("Dockerfile.", toCheck[0].Header);
        Assert.Equal("Signed-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[0].Body);
    }
    
    [Fact]
    public void TestGetConventionalCommitsBothTags()
    {
        var repository = LoadLocalRepository();
        var tagToEndWith = repository.Tags["0.0.1"];
        var tagToStartFrom = repository.Tags["0.0.2"];
        
        var toCheck = LoadLocalRepository()
            .GetConventionalCommits(tagToStartFrom.Target.Sha, tagToEndWith.Target.Sha)
            .ConvertConventional()
            .ToList();

        Assert.Equal(4, toCheck.Count);
        
        Assert.Equal("feat", toCheck[0].Type);
        Assert.Equal("lib-cc", toCheck[0].SubType);    
        Assert.Equal("RepositoryExtension GetConventionalCommits.", toCheck[0].Header);
        Assert.Equal("First naive implementation.\n\nSigned-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[0].Body);
        
        Assert.Equal("feat", toCheck[1].Type);
        Assert.Equal("lib-cc", toCheck[1].SubType);    
        Assert.Equal("Initial repository extension class test.", toCheck[1].Header);
        Assert.Equal("Signed-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[1].Body);
        
        Assert.Equal("hotfix", toCheck[2].Type);
        Assert.Equal("lib-cc", toCheck[2].SubType);    
        Assert.Equal("Missing Conventional.cs file", toCheck[2].Header);
        Assert.Equal("Signed-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[2].Body);
        
        Assert.Equal("feat", toCheck[3].Type);
        Assert.Equal("lib-cc", toCheck[3].SubType);    
        Assert.Equal("Created initial commit parser and test.", toCheck[3].Header);
        Assert.Equal("Including a body in this text to make sure it's around!\n\nSigned-off-by: Johannes Tegnér <johannes@jitesoft.com>", toCheck[3].Body);
    }
}
