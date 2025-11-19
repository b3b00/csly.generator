using System;

namespace csly.models
{

    [AttributeUsage(AttributeTargets.Field)]
    public class PopAttribute : Attribute
    {
        public PopAttribute()
        {
        }
    }
}