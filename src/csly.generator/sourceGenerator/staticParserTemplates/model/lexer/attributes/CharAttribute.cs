using System;

namespace <#NS#>;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class CharacterAttribute : LexemeAttribute
{
    public CharacterAttribute(string delimiter = "'", string escape = "\\") : base(GenericToken.Char, delimiter, escape)
    {   
    } 
}