using csly.generator.model.parser.grammar;

namespace csly.generator.model.parser.grammar;

public class NonTerminalClause : AbstractClause
{
    public NonTerminalClause(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}