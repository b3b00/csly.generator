using jsonparser.JsonModel;
using System;
using System.IO;
using System.Text.Json.Nodes;

namespace jsonparser;

public static class Program
{

    public static void Main(string[] args)
    {
        Console.WriteLine("JSON Parser Sample");
        EbnfJsonGenericParser instance = new EbnfJsonGenericParser();
        EbnfJsonGenericParserMain ebnfJsonGenericParserMain = new EbnfJsonGenericParserMain(instance);
        var content = File.ReadAllText(args[0]);
        var result = ebnfJsonGenericParserMain.Parse(content);
        if (result.IsOk)
        {
            Console.WriteLine("Parsing succeeded.");
            var json = result.Result;
            if (json is JList jsonList)
            {
                Console.WriteLine($"{jsonList.Count} array");
            }
            else if (json is JObject jsonObject)
            {
                Console.WriteLine($"{jsonObject.GetDepth()} deep");
            }
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
