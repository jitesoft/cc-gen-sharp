namespace Jitesoft.CcGen.Format;

public class GenericFormatter : IFormatter
{
    private readonly ICommitFormatter _commitFormatter;
    private readonly ITypeFormatter _typeFormatter;
    private readonly Config _config;

    public GenericFormatter(ICommitFormatter commitFormatter, ITypeFormatter typeFormatter, Config config)
    {
        _commitFormatter = commitFormatter;
        _typeFormatter = typeFormatter;
        _config = config;
    }

    public string FormatCommits(IDictionary<string, IEnumerable<Conventional>> commits)
    {
        var stringWriter = new StringWriter();
        foreach (var val in commits)
        {
            var commitsAsString = FormatCommitList(
                (
                    _config.GroupBreakingChanges ?
                        val.Value.Where(c => !c.Breaking) :
                        val.Value
                ).OrderByDescending(c => c.Commit.Committer.When).ToList()
            );

            if (commitsAsString.Trim().Length > 0)
            {
                stringWriter.WriteLine(_typeFormatter.FormatType(val.Key));
                stringWriter.Write(commitsAsString);
                stringWriter.WriteLine();
            }
        }

        if (_config.GroupBreakingChanges)
        {
            stringWriter.Write(
                FormatBreakingChanges(commits)
            );
        }

        return stringWriter.ToString();
    }

    private string FormatBreakingChanges(IDictionary<string, IEnumerable<Conventional>> commits)
    {
        var stringWriter = new StringWriter();

        if (commits.Any(l => l.Value.Any(c => c.Breaking)))
        {
            stringWriter.WriteLine(_config.GroupedBreakingHeader);
            stringWriter.WriteLine();

            foreach (var val in commits)
            {
                var commitsAsString = FormatCommitList(
                    val.Value.Where(c => c.Breaking)
                        .OrderByDescending(c => c.Commit.Author.When)
                        .ToList()
                );


                if (commitsAsString.Trim().Length > 0)
                {
                    stringWriter.WriteLine(_typeFormatter.FormatType(val.Key));
                    stringWriter.Write(commitsAsString);
                    stringWriter.WriteLine();
                }
            }
        }

        return stringWriter.ToString();
    }


    private string FormatCommitList(IEnumerable<Conventional> commits)
    {
        var stringWriter = new StringWriter();

        foreach (var commit in commits)
        {
            stringWriter.WriteLine(
                _commitFormatter.FormatCommit(commit, commit.Breaking && !_config.GroupBreakingChanges)
            );
        }

        return stringWriter.ToString();
    }
}
