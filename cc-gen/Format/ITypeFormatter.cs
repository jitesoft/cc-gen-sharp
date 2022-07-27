namespace Jitesoft.CcGen.Format;

public interface ITypeFormatter
{
    /// <summary>
    /// Format a string-value type into the intended output format.
    /// </summary>
    /// <param name="type">Type as string.</param>
    /// <returns>String formatted ready for output.</returns>
    public string FormatType(string type);
}
