namespace csly.generator.sourceGenerator;

public static class StringExtensions
{
       public static string TrimQuotes(this string str)
    {
        if (str.StartsWith("\""))
        {
            str = str.Substring(1);
        }
        if (str.EndsWith("\""))
        {
            str = str.Substring(0, str.Length - 1);
        }
        str = str.Replace("\\\"", "\"");
        return str;
    }
}
