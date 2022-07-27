namespace Jitesoft.CcGen.Format;

public interface ICommitFormatter
{
    /// <summary>
    /// Format a specific commit into a string value.
    /// </summary>
    /// <param name="commit">Commit data.</param>
    /// <param name="breaking">If commit is a breaking change.</param>
    /// <returns></returns>
    public string FormatCommit(Conventional commit, bool breaking = false);
}
