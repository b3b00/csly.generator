using csly.generator.model.lexer;
using System.Collections.Generic;

namespace csly.generator.model.parser.grammar;

public class Rule
{
    public string Head { get; set; }
    
    public List<IClause> Clauses { get; set; }

    public List<string> Leaders { get; set; } = new List<string>();

    public string Name => _name;
    
    private readonly string _name;

    private readonly string _methodName;

    public string MethodName => _methodName;

    public Rule(string head, List<IClause> clauses, string methodName)
    {
        this.Head = head;
        this.Clauses = clauses;
        _name = head;
        _methodName = methodName;
    }

    public override string ToString()
    {
        return $"{Head} -> {string.Join(" ", Clauses)}";
    }




}