using System;
using System.Collections.Generic;

namespace csly.models
{

    public class ManySyntaxNode<IN, OUT> : SyntaxNode<IN, OUT> where IN : struct, Enum
    {
        public ManySyntaxNode(string name) : base(name, new List<ISyntaxNode<IN, OUT>>())
        {
        }

        public bool IsManyTokens { get; set; }

        public bool IsManyValues { get; set; }

        public bool IsManyGroups { get; set; }

        public override bool IsEpsilon => Children.Count == 0;


        public void Add(ISyntaxNode<IN, OUT> child)
        {
            Children.Add(child);
        }
    }
}