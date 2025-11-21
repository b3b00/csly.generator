using System;
using System.Diagnostics.CodeAnalysis;

namespace ebnf.grammar
{
    public sealed class ZeroOrMoreClause : ManyClause
    {
        
        public override string Name { get => "ZeroOrMore"+Clause.Name; set { } }

        public ZeroOrMoreClause(IClause clause)
        {
            Clause = clause;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return Clause + "*";
        }

        public override bool MayBeEmpty()
        {
            return true;
        }

        public override string Dump()
        {
            return $"{Clause.Dump()}*";
        }
    }
}