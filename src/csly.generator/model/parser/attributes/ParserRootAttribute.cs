using System;

namespace csly.generator.model.parser.attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ParserRootAttribute : Attribute
{
    
    public string RootRule { get; set; }

    public ParserRootAttribute(string rootRule)
    {
        RootRule = rootRule;
    }

    
}