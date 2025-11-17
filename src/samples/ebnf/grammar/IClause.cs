namespace ebnf.grammar;

public interface IClause
{
    string Name { get; set; }
    bool MayBeEmpty();
}