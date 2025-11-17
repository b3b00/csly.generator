using Microsoft.CodeAnalysis.FlowAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ebnf.grammar;

public class TerminalClause : AbstractClause
{

    private bool _isExplicit;
    public bool Discarded { get; set; }

    public bool IsExplicit => _isExplicit;
    public TerminalClause(string name, bool discarded = false)
    {
        Name = name;
        if (Name.StartsWith("'") && Name.EndsWith("'"))
        {
            _isExplicit = true;
            Name = Name.Substring(1, Name.Length - 2);
        }
        Discarded = discarded;
    }

    public override string ToString()
    {
        return _isExplicit ? $"'{Name}'" : Name;
    }

    public override bool MayBeEmpty() => false;

    


}

public sealed class IndentTerminalClause : TerminalClause
{
    private IndentationType ExpectedIndentation;

    public IndentTerminalClause(IndentationType expectedIndentation, bool discard) : base(IndentationType.Indent.ToString())
    {
        ExpectedIndentation = expectedIndentation;
        Discarded = discard;
    }

    public override bool MayBeEmpty()
    {
        return false;
    }


    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var b = new StringBuilder();
        b.Append(ExpectedIndentation == IndentationType.Indent ? "TAB" : "UNTAB");
        if (Discarded) b.Append("[d]");
        return b.ToString();
    }

}