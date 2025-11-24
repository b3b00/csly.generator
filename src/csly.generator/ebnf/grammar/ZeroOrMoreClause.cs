using System;
using System.Diagnostics.CodeAnalysis;

namespace ebnf.grammar
{
    public sealed class ZeroOrMoreClause : ManyClause
    {
        
        public override string Name { get => $"ZeroOrMore_{manyClause.Name}"; set { } }

        public ZeroOrMoreClause(IClause clause)
        {
            manyClause = clause;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return manyClause + "*";
        }

        public override bool MayBeEmpty()
        {
            return true;
        }

        public override string Dump()
        {
            return $"{manyClause.Dump()}*";
        }
    }
}