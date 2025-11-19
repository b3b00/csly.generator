namespace ebnf.grammar
{

    public abstract class AbstractClause : IClause
    {
        public string Name { get; set; }

        public abstract bool MayBeEmpty();
    }
}