using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;


namespace ebnf.grammar;

public class Rule : GrammarNode
{
    public Rule()
    {
        Clauses = new List<IClause>();
        VisitorMethodsForOperation = new Dictionary<string, OperationMetaData>();
        LambdaVisitorsForOperation = new Dictionary<string, OperationMetaData>();
        MethodName = null;
        IsSubRule = false;
        NodeName = "";
    }

    public string NodeName { get; set; } = null;
    
    public string[] SubNodeNames { get; set; } = null;
    
    private Dictionary<string,string> NodeNamesMap { get; set; } = new Dictionary<string, string>();
    
    public bool IsByPassRule { get; set; } = false;

    // visitors for operation rules
    private Dictionary<string, OperationMetaData> VisitorMethodsForOperation { get; }
    
    private Dictionary<string, OperationMetaData> LambdaVisitorsForOperation { get; }
    
    // visitor for classical rules
    private string MethodName { get; set; }
    
    public bool IsExpressionRule { get; set; }
    
    public bool IsInfixExpressionRule { get; set; }

    public Affix ExpressionAffix { get; set; }

    public string RuleString { get; set;  }

    public List<IClause> Clauses { get; set; }
    public List<string> Leaders { get; set; } = new List<string>();

    public string NonTerminalName { get; set; }

    public bool ContainsSubRule
    {
        get
        {
            if (Clauses != null && Clauses.Any())
            {
                bool contains = false;
                foreach (var clause in Clauses)
                {
                    switch (clause)
                    {
                        case GroupClause _:
                            contains = true;
                            break;
                        case ManyClause many:
                            contains  |=  many.Clause is GroupClause;
                            break;
                        case OptionClause option:
                            contains  |=  option.Clause is GroupClause;
                            break;
                    }

                    if (contains)
                    {
                        return true;
                    }
                    }
                }

            return false;
        }
    }

    public bool IsSubRule { get; set; }

    public bool MayBeEmpty => Clauses == null
                              || Clauses.Count == 0
                              || Clauses.Count == 1 && Clauses[0].MayBeEmpty();

    public bool ForcedName { get; set; }


    public OperationMetaData GetOperation(string token)
    {
        OperationMetaData operation = null;
        if (IsExpressionRule)
        {
            if (LambdaVisitorsForOperation.ContainsKey(token))
            {
                return LambdaVisitorsForOperation[token];
            }

            if (VisitorMethodsForOperation.ContainsKey(token))
            {
                return VisitorMethodsForOperation[token];
            }
            else
            {
                ;

            }
        }

        return operation;
    }
    
    public List<OperationMetaData> GetOperations()
    {
        if (IsExpressionRule)
        {
            return VisitorMethodsForOperation.Values.ToList();
        }

        return null;
    }

    
    public string GetVisitorMethod(string token = null)
    {
        string visitor = null;
        if (IsExpressionRule && !string.IsNullOrEmpty(token))
        {
            var operation = VisitorMethodsForOperation.TryGetValue(token, out var value)
                ? value
                : null;
            visitor = operation?.MethodName;
        }
        else
        {
            visitor = MethodName;
        }

        return visitor;
    }
    
    
    
    public void SetVisitor(string methodName)
    {
        MethodName = methodName;
    }

    public void SetVisitor(OperationMetaData operation)
    {
        NodeNamesMap[operation.Operatorkey] = operation.NodeName;
        if (operation.MethodName != null)
            VisitorMethodsForOperation[operation.Operatorkey] = operation;
        
    }

    
    
    
}