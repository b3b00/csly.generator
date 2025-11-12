using System;

namespace csly.generator.model.lexer.attributes;

public enum DateFormat
{
    YYYYMMDD,
    DDMMYYYY
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class DateAttribute : LexemeAttribute
{
    public DateAttribute(DateFormat format = DateFormat.DDMMYYYY, char separator = '-', int channel = Channels.Main) : base(GenericToken.Date,
        channel: channel, parameters: new[] { format.ToString(), separator.ToString() })
    {
    }
}