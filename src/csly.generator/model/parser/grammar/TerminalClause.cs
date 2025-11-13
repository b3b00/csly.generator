namespace csly.generator.model.parser.grammar;

public class TerminalClause : AbstractClause
{

    private bool _isExplicit;

    public bool IsExplicit => _isExplicit;
    public TerminalClause(string name)
    {
        Name = name;
        if (Name.StartsWith("'") && Name.EndsWith("'"))
        {
            _isExplicit = true;
            Name = Name.Substring(1, Name.Length - 2);
        }
    }

    public override string ToString()
    {
        return _isExplicit ? $"'{Name}'" : Name;
    }


}