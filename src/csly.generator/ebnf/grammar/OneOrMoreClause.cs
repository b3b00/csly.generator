using System;
using System.Diagnostics.CodeAnalysis;

namespace ebnf.grammar
{

    public sealed class OneOrMoreClause : ManyClause
    {

        public override string Name { get => $"OneOrMore_{manyClause.Name}"; set { } }

        public OneOrMoreClause(IClause clause)
        {
            manyClause = clause;
        }


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return manyClause + "+";
        }

        public override bool MayBeEmpty()
        {
            return true;
        }

        public override string Dump()
        {
            return $"{manyClause.Dump()}+";
        }
    }
}