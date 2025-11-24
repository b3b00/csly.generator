// See https://aka.ms/new-console-template for more information
using extending;

Console.WriteLine("Hello, World!");

var instance = new ExtParser();
var main = new ExtParserMain(instance);
var result = main.Parse(" Y Z A B B B D C");
if (result.IsOk)
{
    Console.WriteLine("Parsed: " + result.Result);
}
else
{
    Console.WriteLine("Errors: " + string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
}