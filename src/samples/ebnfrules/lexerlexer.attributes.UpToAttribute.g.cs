using System;

namespace csly.models;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class UpToAttribute : LexemeAttribute
{
    public UpToAttribute(params string[] exceptions) : base(GenericToken.UpTo, exceptions)
    {
    }
}