using System;

namespace <#NS#>;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SubNodeNamesAttribute : Attribute
{
    public  string[] Names { get;  } = null;

    public SubNodeNamesAttribute(params string[] names)
    {
        Names = names;
    }
}