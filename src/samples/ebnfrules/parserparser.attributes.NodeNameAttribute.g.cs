using System;

namespace csly.models;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NodeNameAttribute : Attribute
{
    public  string Name { get;  } = null;

    public NodeNameAttribute(string name)
    {
        Name = name;
    }
}