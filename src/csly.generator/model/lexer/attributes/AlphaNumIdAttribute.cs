using System;

namespace csly.generator.model.lexer.attributes;


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class AlphaNumIdAttribute : LexemeAttribute
{
    public AlphaNumIdAttribute() : base(GenericToken.Identifier,IdentifierType.AlphaNumeric)
    {
        
    } 
}