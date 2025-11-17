using System;
using System.Diagnostics.CodeAnalysis;

namespace ebnf.grammar;

public sealed class OneOrMoreClause : ManyClause
{
    public OneOrMoreClause(IClause clause)
    {
        Clause = clause;
    }


    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return Clause + "+";
    }

    public override bool MayBeEmpty()
    {
        return true;
    }

}