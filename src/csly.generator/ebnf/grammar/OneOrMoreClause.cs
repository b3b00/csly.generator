using System;
using System.Diagnostics.CodeAnalysis;

namespace ebnf.grammar
{

    public sealed class OneOrMoreClause : ManyClause
    {

        public override string Name { get => "OneOrMore" + Clause.Name; set { } }

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

        public override string Dump()
        {
            return $"{Clause.Dump()}+";
        }
    }
}