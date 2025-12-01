using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expr
{
    public  class Program
    {

        private static void ParseExpr(string source, int expecting = -666)
        {
            ExprParser exprInstance = new ExprParser();
            ExprParserMain exprMain = new ExprParserMain(exprInstance);

            var exprParsed = exprMain.Parse(source);
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
            var instance = new ExprParser();
            var main = new ExprParserMain( instance);
            ParseExpr("1 +1", 2);
            ParseExpr("1 + 2 * 3", 7);
            ParseExpr("-1 + 2 * 3", 5);
            ParseExpr("2 * -1", -2);
            ParseExpr("10 / 2 + 3", 8);
            ParseExpr("10 / (2 + 3)", 2);
            ParseExpr("-1 + -1", -2);
            ParseExpr("5!", 5 * 4 * 3 * 2 * 1);
            ParseExpr("-5!", -5 * 4 * 3 * 2 * 1);

        }
    }
}
