// See https://aka.ms/new-console-template for more information

using TestNuget;
using TestNuget.theparser;

public class Program
{
    public static void Main(string[] args)
    {
        TheParser instance =  new TheParser();
        TheParserMain parser = new TheParserMain(instance);

        var result = parser.Parse("sum : 1 2 3 4 5 6 7 8 9 10");
        if (result.IsOk)
        {
            Console.WriteLine("parse OK ! ");
            Console.WriteLine(result.Result);
        }
        else 
        {
            Console.WriteLine("parse FAIL ! ");
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error);
            }
        }
        
    }
}