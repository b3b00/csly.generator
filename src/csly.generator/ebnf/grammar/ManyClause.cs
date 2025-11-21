using System;

namespace ebnf.grammar
{

    public abstract class ManyClause : IClause
    {
        public IClause Clause { get; set; }
        public virtual string Name { get => Clause.Name; set { } }

        public abstract bool MayBeEmpty();

        public abstract string Dump();

    }
}