using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Field)]
    public class PushAttribute : Attribute
    {
        public string TargetMode { get; }

        public PushAttribute(string mode)
        {
            TargetMode = mode;
        }
    }
}