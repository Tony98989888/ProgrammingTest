
using System.Linq;
using System.Text.RegularExpressions;

public static class DialogEditorStringHelper
{
    public static string FormatText(string source, bool letterOnly = true) 
    {
        // Trim white space
        var result = Regex.Replace(source, @"\s+", string.Empty);
        if (letterOnly) 
        {
            result = result.Where(c => char.IsLetterOrDigit(c)).ToArray().ToString();
        }
        return result;
    }
}
