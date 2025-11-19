using System;

namespace ebnf.grammar
{

    public abstract class ManyClause : IClause
    {
        public IClause Clause { get; set; }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public abstract bool MayBeEmpty();

    }
}