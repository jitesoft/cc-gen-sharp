namespace Jitesoft.Libs.ConventionalCommits;

/// <summary>
/// Record with conventional commit data.
/// 
/// The commit is always directly connected to a LibGit2Sharp.Commit and contains
/// no references to the actual commit or tree, it's just an extracted record of the message.
/// </summary>
public record Conventional
{
    /// <summary>
    /// Conventional Type.
    /// This is the initial value in the conventional commit:
    ///
    /// Type: {header}
    /// </summary>
    public string Type { get; init; } = null!;
    
    /// <summary>
    /// Conventional SubType.
    /// This is an optional subtype in the conventional commit:
    ///
    /// Type(subType): {header}
    /// </summary>
    public string SubType { get; init; } = null!;
    
    /// <summary>
    /// The header message of the conventional commit.
    /// </summary>
    public string Header { get; init; } = null!;
    
    /// <summary>
    /// The body of the conventional commit.
    /// Comes after the header and a separating new-line.
    /// </summary>
    public string Body { get; init; } = null!;
}
