namespace Jitesoft.CcGen.Format;

public interface IFormatter
{

    /// <summary>
    /// Formats a commit list to expected output format.
    ///
    /// Commits are passed as a dictionary where the key is the type as string
    /// and the value is a list of commits under the specific type.
    /// </summary>
    /// <param name="commits">Commits.</param>
    /// <returns>Formatted string of all commits in the group structure.</returns>
    public string FormatCommits(IDictionary<string, IEnumerable<Conventional>> commits);
}
