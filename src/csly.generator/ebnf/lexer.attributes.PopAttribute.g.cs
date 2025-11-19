using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Field)]
    public class PopAttribute : Attribute
    {
        public PopAttribute()
        {
        }
    }
}