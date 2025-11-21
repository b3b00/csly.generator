using System;
using System.Collections.Generic;
using System.Text;




namespace csly.generator.sourceGenerator
{
    internal class GeneratorLogger
    {

        public static void Log(string message)
        {
#if DEBUG
            System.IO.File.AppendAllText("c:/tmp/generation/parsergeneration.log", message);
#endif
        }

    }
}
