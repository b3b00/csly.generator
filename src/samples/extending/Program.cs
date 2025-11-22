// See https://aka.ms/new-console-template for more information
using extending;

Console.WriteLine("Hello, World!");

var instance = new ExtParser();
var main = new ExtParserMain(instance);
var result = main.Parse("A B B B C");
if (result.IsOk)
{
    Console.WriteLine("Parsed: " + result.Result);
}
else
{
    Console.WriteLine("Errors: " + string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
}