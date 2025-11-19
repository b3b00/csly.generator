using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BroadenTokenWindowAttribute : Attribute
    {
        public BroadenTokenWindowAttribute()
        {

        }
    }
}