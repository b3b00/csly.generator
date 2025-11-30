using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expr
{
    public  class Program
    {

        private static void ParseExpr(string source, string expecting = null)
        {
            ExprParser exprInstance = new ExprParser();
            ExprParserMain exprMain = new ExprParserMain(exprInstance);

            var exprParsed = exprMain.Parse(source);
            if (exprParsed.IsOk)
            {
                Console.WriteLine($"\x1b[34mParsing [{source}] succeeded.\x1b[0m");
                Console.WriteLine($"\x1b[34mParsed value: {exprParsed.Result}\x1b[0m");
                if (expecting == null)
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
            //ParseExpr("1 +1", "((1 + 1))");
            ParseExpr("1 + 2 * 3", "((1 + (2 * 3)))");

        }
    }
}
