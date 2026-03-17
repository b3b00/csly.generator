namespace ebnf.grammar
{

    public abstract class AbstractClause : IClause
    {
        public virtual string Name { get; set; }

        public abstract string Dump();

        public abstract bool MayBeEmpty();
        public GrammarNode Parent { get; set; }
        public bool IsRoot => Parent == null;
        
        public GrammarNode Root =>  IsRoot ? this : Parent.Root;
    }
}