using System.Reflection;
using System;
using System.Collections.Generic;

namespace <#NS#>;

public class OptionSyntaxNode<IN, OUT> : SyntaxNode<IN, OUT> where IN : struct, Enum
{
    public bool IsGroupOption { get; set; } = false;

    public OptionSyntaxNode(string name, List<ISyntaxNode<IN, OUT>> children = null, string visitor = null) : base(
        name, children, visitor)
    { }
}