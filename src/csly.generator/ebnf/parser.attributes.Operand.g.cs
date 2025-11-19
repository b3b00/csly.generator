using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OperandAttribute : Attribute
    {
    }
}