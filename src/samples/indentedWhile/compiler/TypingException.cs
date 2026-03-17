using System;

namespace csly.indented.whileLang.compiler
{
    public class TypingException : Exception
    {
        public TypingException(string message) : base(message)
        {
        }
    }
}