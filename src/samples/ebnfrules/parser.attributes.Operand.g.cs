using System;

namespace csly.models
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OperandAttribute : Attribute
    {
    }
}