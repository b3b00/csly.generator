using System;

namespace csly.models
{


    [AttributeUsage(AttributeTargets.Enum)]
    public class LexerAttribute : Attribute
    {
        private enum DefaultEnum : int
        {
        }

        private bool? ignoreWS;

        private bool? ignoreEOL;

        private char[] whiteSpace;

        private bool? keyWordIgnoreCase;


        public bool IgnoreWS { get; set; } = true;

        public bool IgnoreEOL { get; set; } = true;

        public char[] WhiteSpace { get; set; } = new[] { ' ', '\t', '\r', '\n' };

        public bool KeyWordIgnoreCase { get; set; } = true;


        public bool IndentationAWare { get; set; } = false;

        public string Indentation { get; set; } = "";

    }
}