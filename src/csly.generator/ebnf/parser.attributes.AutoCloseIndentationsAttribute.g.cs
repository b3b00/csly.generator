using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoCloseIndentationsAttribute : Attribute
    {
        public AutoCloseIndentationsAttribute()
        {

        }
    }
}