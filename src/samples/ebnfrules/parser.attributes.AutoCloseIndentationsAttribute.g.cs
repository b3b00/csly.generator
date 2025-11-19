using System;

namespace csly.models
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoCloseIndentationsAttribute : Attribute
    {
        public AutoCloseIndentationsAttribute()
        {

        }
    }
}