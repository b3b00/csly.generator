using System;

namespace <#NS#>;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ParserRootAttribute : Attribute
{
    
    public string RootRule { get; set; }

    public ParserRootAttribute(string rootRule)
    {
        RootRule = rootRule;
    }

    
}