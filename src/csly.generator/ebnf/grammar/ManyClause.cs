using System;

namespace ebnf.grammar
{

    public abstract class ManyClause : IClause
    {
        public GrammarNode Parent { get; set; }
        public bool IsRoot => Parent == null;
        
        public GrammarNode Root =>  IsRoot ? Root : Parent.Root;
        
        public IClause manyClause { get; set; }
        public virtual string Name { get => manyClause.Name; set { } }

        public abstract bool MayBeEmpty();

        public abstract string Dump();

    }
}