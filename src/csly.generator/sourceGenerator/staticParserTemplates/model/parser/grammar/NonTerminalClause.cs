namespace <#NS#>;

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