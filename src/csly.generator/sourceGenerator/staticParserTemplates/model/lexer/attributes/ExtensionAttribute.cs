using System;

namespace <#NS#>;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ExtensionAttribute : LexemeAttribute
{
    public ExtensionAttribute() : base(GenericToken.Extension)
    {
        
    } 
}