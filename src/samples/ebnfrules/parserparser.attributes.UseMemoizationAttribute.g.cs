using System;

namespace csly.models;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class UseMemoizationAttribute : Attribute
{
    public UseMemoizationAttribute()
    {
            
    }
}