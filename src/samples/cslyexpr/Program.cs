// See https://aka.ms/new-console-template for more information
using expr;
using sly.parser.generator;

var instance = new ExprParser();

ParserBuilder<ExprToken, int> builder = new ParserBuilder<ExprToken, int>();
var parserResult = builder.BuildParser(instance, ParserType.EBNF_LL_RECURSIVE_DESCENT);
if (parserResult.IsOk)
{
    var parser = parserResult.Result;
    var parsed = parser.Parse("1 + 1","root");
    if (parsed.IsError)
    {
        Console.WriteLine("Parse failed");
        foreach (var err in parsed.Errors)
        {
            Console.WriteLine(err.ContextualErrorMessage);
        }
        return;
    }
    Console.WriteLine($"Parse result: {parsed.Result}");
    var tree = parsed.SyntaxTree;
    Console.WriteLine(tree.Dump("  "));
}
else
{
    Console.WriteLine("Parser build failed");
    foreach (var err in parserResult.Errors)
    {
        Console.WriteLine(err.Message);
    }
}

