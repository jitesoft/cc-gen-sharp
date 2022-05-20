using System;
using Jitesoft.Libs.ConventionalCommits;
using LibGit2Sharp;
using Moq;
using Xunit;

namespace lib_cc.tests;

public class CommitExtensionTest
{
    [Fact]
    public void TestConventionalSuccess()
    {
        var commitMessage = @"feat(test): this is the header

This is the body
Which is multi line!";

        var mock = new Mock<Commit>();
        mock.SetupGet(x => x.Message).Returns(commitMessage);

        var commit = mock.Object;
        
        Assert.True(commit.IsConventional());

        var success = commit.ParseConventional(out var result);
        Assert.True(success);
        
        Assert.Equal("feat", result.Type);
        Assert.Equal("test", result.SubType);
        Assert.Equal("this is the header", result.Header);
        Assert.Equal(@"This is the body
Which is multi line!", result.Body);
    }
    
    [Fact]
    public void TestConventionalSuccessNoSubType()
    {
        var commitMessage = @"feat: this is the header

This is the body
Which is multi line!";


        var mock = new Mock<Commit>();
        mock.SetupGet(x => x.Message).Returns(commitMessage);

        var commit = mock.Object;
        
        Assert.True(commit.IsConventional());
        
        var success = commit.ParseConventional(out var result);
        
        Assert.True(success);
        Assert.Equal("feat", result.Type);
        Assert.Equal("", result.SubType);
        Assert.Equal("this is the header", result.Header);
        Assert.Equal(@"This is the body
Which is multi line!", result.Body);
    }
    
    [Fact]
    public void TestConventionalSuccessNoBody()
    {
        var commitMessage = @"feat: this is the header";


        var mock = new Mock<Commit>();
        mock.SetupGet(x => x.Message).Returns(commitMessage);

        var commit = mock.Object;
        
        Assert.True(commit.IsConventional());
        
        var success = commit.ParseConventional(out var result);
        
        Assert.True(success);
        
        Assert.Equal("feat", result.Type);
        Assert.Equal("", result.SubType);
        Assert.Equal("this is the header", result.Header);
        Assert.Equal("", result.Body);
    }
    
    [Fact]
    public void TestIsNotConventional()
    {        
        var commitMessage = @"This is not conventional

This is the body
Which is multi line!";
        
        var mock = new Mock<Commit>();
        mock.SetupGet(x => x.Message).Returns(commitMessage);

        var commit = mock.Object;
        
        Assert.False(commit.IsConventional());
        var success = commit.ParseConventional(out var result);
        
        Assert.False(success);
        Assert.Null(result);
        
    }
}
