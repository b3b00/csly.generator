using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expr
{
    public  class Program
    {

        private static void ParseExpr2(params (string source, int expecting)[] testCase)
        {
            StringBuilder source = new StringBuilder();
            source.AppendLine("GO : HEAD( AA 1, BB 1*2, CC 5-2 )");
            char argId = (char)(((byte)'A') - 1);
            for (int i = 0; i < testCase.Length; i++)
            {
                argId = (char)(((byte)argId) + 1);

                source.Append($"TEST({argId},{testCase[i].source},{Math.Abs(testCase[i].expecting)}, {(testCase[i].expecting < 0 ? "TRUE" : "FALSE")}  )");
                if (i % 2 == 0)
                {
                    source.Append(";");
                }
                source.AppendLine();
            }
            argId = (char)(((byte)'A') - 1);
            for (int i = 0; i < testCase.Length; i++)
            {
                argId = (char)(((byte)argId) + 1);
                source.Append($"PRINT {argId}");
                if (i % 2 == 0)
                {
                    source.Append(";");
                }
                source.AppendLine();
            }

            var program = source.ToString();
            Console.WriteLine("[[");
            Console.WriteLine(program);
            Console.WriteLine("]]");
            ExprParser exprInstance = new ExprParser();
            ExprParserMain exprMain = new ExprParserMain(exprInstance);

            var expecting = testCase.Select(tc => tc.expecting).Sum();

            var exprParsed = exprMain.Parse(program);
            if (exprParsed.IsOk)
            {
                Console.WriteLine($"\x1b[34mParsing [{source}] succeeded.\x1b[0m");
                Console.WriteLine($"\x1b[34mParsed value: {exprParsed.Result}\x1b[0m");
                if (expecting == -666)
                {
                    return;
                }
                if (exprParsed.Result != expecting)
                {
                    Console.WriteLine($"\x1b[31mfound >>{exprParsed.Result}<<  But was expecting: >>{expecting}<<\x1b[0m");
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
                foreach (var error in exprParsed.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return;
        }

        public static void Main(string[] args)
        {

            Console.WriteLine("============= EXPRESSION PARSER TESTER =============");

           

            ParseExpr2
            (("1 + (2 + 3)", 6)
            , ("1 - 2 - 3", -4)
            , ("1 + 2 * 3", 7)
            , ("-1 + 2 * 3", 5)
            , ("2 * -1", -2)
            , ("10 / 2 + 3", 8)
            , ("10 / (2 + 3)", 2)
            , ("-1 + -1", -2)
            , ("5!", 120)
            , ("-5!", -120)
            );

            return;
            

        }
    }
}
