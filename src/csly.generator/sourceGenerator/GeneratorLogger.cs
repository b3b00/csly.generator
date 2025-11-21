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
            string timedMessage = $"[{DateTime.Now.ToString("o")}] {message}{Environment.NewLine}";
            System.IO.File.AppendAllText("c:/tmp/generation/parsergeneration.log", timedMessage);
#endif
        }

    }
}
