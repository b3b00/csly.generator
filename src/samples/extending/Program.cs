// See https://aka.ms/new-console-template for more information
using extending;


void ParseToky(string source, string expecting = null)
{
    ExtParser tokyInstance = new ExtParser();
    ExtParserMain tokyMain = new ExtParserMain(tokyInstance);

    var tokyParsed = tokyMain.Parse(source);
    if (tokyParsed.IsOk)
    {
        Console.WriteLine($"\x1b[34mParsing [{source}] succeeded.\x1b[0m");
        Console.WriteLine($"\x1b[34mParsed value: {tokyParsed.Result}\x1b[0m");
        if (expecting == null)
        {
            return;
        }
        if (tokyParsed.Result != expecting)
        {
            Console.WriteLine($"\x1b[31mfound >>{tokyParsed.Result}<<  But was expecting: >>{expecting}<<\x1b[0m");
        }
        else
        {
            Console.WriteLine($"\x1b[32mGot expected value: >>{expecting}<<\x1b[0m");
        }
    }
    else
    {
        Console.WriteLine($"Parsing [{source}] failed.");
        Console.WriteLine("Errors:");
        foreach (var error in tokyParsed.Errors)
        {
            Console.WriteLine(error.ErrorMessage);
        }
    }
    return;
}

ParseToky("AA B > A X AA X < > X A <");
Console.WriteLine();
Console.WriteLine("---------------------------------------------------");
Console.WriteLine();
ParseToky("AA B > A X AA X < > <");