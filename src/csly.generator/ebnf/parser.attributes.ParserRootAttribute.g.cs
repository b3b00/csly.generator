using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ParserRootAttribute : Attribute
    {

        public string RootRule { get; set; }

        public ParserRootAttribute(string rootRule)
        {
            RootRule = rootRule;
        }
    }
}