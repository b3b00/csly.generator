using System;

namespace csly.generator.model.lexer;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class TokenCallbackAttribute : Attribute
{
    public int EnumValue { get; set; }

    public TokenCallbackAttribute(int enumValue)
    {
        EnumValue = enumValue;
    }
}