using csly.models;
using ebnf.grammar;
using System.Diagnostics;

namespace ebnf.grammar;

[ParserGenerator]

public partial class Ebnf : AbstractParserGenerator<EbnfTokenGeneric, RuleParser, GrammarNode>
{

}

public class Program
{
    public static void Main(string[] args)
    {
        RuleParser parser = new RuleParser(new List<string>() { "A","B","C","D","E"});
        RuleParserMain main = new RuleParserMain(parser);
        var t = main.Parse("a : b C d E f");
        if (t.IsOk)
        {
            Console.WriteLine("Parsing succeeded.");
            Console.WriteLine(t.Result.dump());
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