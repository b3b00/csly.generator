using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : LexemeAttribute
    {
        public ExtensionAttribute() : base(GenericToken.Extension)
        {

        }
    }
}