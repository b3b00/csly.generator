namespace ebnf.grammar
{

    public interface IClause : GrammarNode
    {
        string Name { get; set; }
        bool MayBeEmpty();

        public GrammarNode Parent { get; set; }

        public bool IsRoot { get; }
    }
}
