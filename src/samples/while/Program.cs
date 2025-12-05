using csly.whileLang.parser;

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

            //WhileParserGenericMain parserMain = new WhileParserGenericMain(instance);


        }
    }   
}
