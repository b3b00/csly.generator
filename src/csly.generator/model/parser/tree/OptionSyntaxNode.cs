using System.Reflection;

namespace csly.generator.model.parser.tree
{
    public class OptionSyntaxNode<IN, OUT> : SyntaxNode<IN, OUT> where IN : struct, Enum
    {
        public bool IsGroupOption { get; set; } = false;

        public OptionSyntaxNode(string name, List<ISyntaxNode<IN, OUT>> children = null, MethodInfo visitor = null) : base(
            name, children, visitor)
        { }
    }
}