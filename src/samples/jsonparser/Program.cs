using System;
using System.IO;

namespace jsonparser;

public static class Program
{

    public static void Main(string[] args)
    {
        Console.WriteLine("JSON Parser Sample");
        EbnfJsonGenericParser instance = new EbnfJsonGenericParser();
        EbnfJsonGenericParserMain ebnfJsonGenericParserMain = new EbnfJsonGenericParserMain(instance);
        var content = File.ReadAllText("test.json");
        var result = ebnfJsonGenericParserMain.Parse(content);
        if (result.IsOk)
        {
            Console.WriteLine("Parsing succeeded.");
            var json = result.Result;
            Console.WriteLine(json.Dump());
        }
        else
        {
            Console.WriteLine("Parsing failed:");
            foreach (var err in result.Errors)
            {
                Console.WriteLine(err.ContextualErrorMessage);
            }
        }
    }
}
