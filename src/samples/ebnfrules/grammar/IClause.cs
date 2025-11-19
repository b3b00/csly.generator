namespace ebnf.grammar
{

    public interface IClause : GrammarNode
    {
        string Name { get; set; }
        bool MayBeEmpty();
    }
}