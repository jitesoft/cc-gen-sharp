using LibGit2Sharp;

namespace Jitesoft.Libs.ConventionalCommits;

/// <summary>
/// Record with conventional commit data.
/// </summary>
public record Conventional
{
    public bool Breaking { get; init; } = false;
    
    /// <summary>
    /// Commit
    /// </summary>
    public Commit Commit { get; init; } = null!;
    
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
