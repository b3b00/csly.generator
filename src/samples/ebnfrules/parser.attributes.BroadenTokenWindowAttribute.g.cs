using System;

namespace csly.models
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BroadenTokenWindowAttribute : Attribute
    {
        public BroadenTokenWindowAttribute()
        {

        }
    }
}