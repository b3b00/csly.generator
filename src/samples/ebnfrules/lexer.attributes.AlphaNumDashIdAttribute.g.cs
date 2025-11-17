using System;

namespace csly.models;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class AlphaNumDashIdAttribute : LexemeAttribute
{
    public AlphaNumDashIdAttribute() : base(GenericToken.Identifier,IdentifierType.AlphaNumericDash)
    {
        
    } 
}