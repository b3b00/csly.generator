using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TokenCallbackAttribute : Attribute
    {
        public int EnumValue { get; set; }

        public TokenCallbackAttribute(int enumValue)
        {
            EnumValue = enumValue;
        }
    }
}