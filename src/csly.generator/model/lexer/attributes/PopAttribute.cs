using System;

namespace csly.generator.model.lexer.attributes;

[AttributeUsage(AttributeTargets.Field)]
public class PopAttribute : Attribute
{
    public PopAttribute()
    {
    }
}