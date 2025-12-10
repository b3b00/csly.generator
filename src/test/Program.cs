// See https://aka.ms/new-console-template for more information
using test;
using System.Globalization;
using interactiveCLI;
using interactiveCLI.forms;

namespace test.the.fucking.generators;

public class Program {

    public static void Main(string[] args) {

        Test();
    }

    public static void Test()
    {
        TestForm form = new TestForm();

        form.Ask();
        while (!string.IsNullOrEmpty(form.Code))
        {
            Parser instance = new Parser();
            ParserMain main = new ParserMain(instance);
            var r = main.Parse(form.Code);
            if (r.IsOk)
            {
                Console.WriteLine("Parsed: " + r.Result);
            }
            else
            {
                Console.WriteLine("Errors:");
                foreach (var err in r.Errors)
                {
                    Console.WriteLine(err.ErrorMessage);
                }
            }
            form.Ask();
        }
    }

    

}

[Form]
public partial class TestForm
{
    [TextArea("code", index:0,maxLines:0)]
    public String Code {get; set;}
}