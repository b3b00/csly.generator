using System;

namespace csly.models;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class DoubleAttribute : LexemeAttribute
{
    public DoubleAttribute(string decimalDelimiter = ".", int channel = Channels.Main) : base(GenericToken.Double,
        channel:channel, parameters:decimalDelimiter)
    {
    }
}