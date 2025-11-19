// See https://aka.ms/new-console-template for more information
using sourceGenerationTester.expressionParser;

Console.WriteLine("Hello, World!");

ExpressionParser parser = new ExpressionParser();
ExpressionParserMain main = new ExpressionParserMain(parser);
var parsed = main.Parse("2+2");
if (parsed.IsOk)
{
    Console.WriteLine("Parsed successfully!");
}
else
{
    Console.WriteLine($"Parsing failed: {parsed.Errors.Count} error{(parsed.Errors.Count > 1 ? "s":"")}");
    foreach (var err in parsed.Errors)
    {
        Console.WriteLine(err.ErrorMessage);
    }
}
