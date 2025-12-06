using csly.whileLang;
using csly.whileLang.interpreter;
using System;

namespace csly.whileLang
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var whileCode = @"
            (
    r:=1;
    i:=1;
    while (i < 11) and true do 
    ( 
      r := r * i;
      print r;
      print i;
      i := i + 1 
    )
)
            ";
            WhileParserGeneric instance = new WhileParserGeneric();

            WhileParserGenericMain parserMain = new WhileParserGenericMain(instance);

            var parseResult = parserMain.Parse(whileCode);

            if (parseResult.IsOk)
            {
                
                Interpreter interpreter = new Interpreter();
                var context = interpreter.Interprete(parseResult.Result as Statement);
                Console.WriteLine("Final Context:");
                Console.WriteLine(context.ToString());
                
            }
            else
            {
                foreach (var err in parseResult.Errors)
                {
                    System.Console.WriteLine(err.ToString());
                }
            }


        }
    }   
}
