using System;

namespace csly.generator.model.lexer.attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ExtensionAttribute : LexemeAttribute
{
    public ExtensionAttribute() : base(GenericToken.Extension)
    {
        
    } 
}