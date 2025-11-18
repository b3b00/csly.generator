namespace ebnf.grammar;

public static class Program
{
    public static void Main(string[] args)
    {
        RuleParser instance = new RuleParser(new List<string>() { "A", "B", "C", "D", "E" });
        RuleParserMain ruleParserMain = new RuleParserMain(instance);

        
        RuleParserMain main = new RuleParserMain(instance);
        var t = main.Parse("a : b C d E f");
        if (t.IsOk)
        {
            var rule = t.Result;
            Console.WriteLine("Parsing succeeded.");
        }
        else
        {
            Console.WriteLine("Parsing failed.");
            foreach (var err in t.Errors)
            {
                Console.WriteLine(err.ErrorMessage);
            }
        }
    }
}