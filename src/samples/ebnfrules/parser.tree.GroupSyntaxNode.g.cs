using System;
using System.Collections.Generic;
using System;

namespace csly.models;

public class GroupSyntaxNode<IN, OUT> : ManySyntaxNode<IN, OUT> where IN : struct, Enum
{
    public GroupSyntaxNode(string name) : base(name)
    {
    }

    public GroupSyntaxNode(string name,  List<ISyntaxNode<IN, OUT>> children) : this(name)
    {
        Children.AddRange(children);
    }

}