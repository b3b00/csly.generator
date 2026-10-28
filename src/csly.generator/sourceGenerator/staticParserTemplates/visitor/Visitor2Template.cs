

namespace <#NS#>;

internal class <#PARSER#>Visitor2
{
    private readonly <#PARSER#> _instance;

    public <#PARSER#>Visitor2(<#PARSER#> instance)    {
        _instance = instance;
    }

private Token<<#LEXER#>> VisitTerminal(SyntaxNode<<#LEXER#>,<#OUTPUT#>> node, int childIndex <#CONTEXT#>) {
            return (node.Children[childIndex] as SyntaxLeaf<<#LEXER#>,<#OUTPUT#>>).Token;
    }

private <#OUTPUT#> VisitNonTerminal(SyntaxNode<<#LEXER#>,<#OUTPUT#>> node, int childIndex <#CONTEXT#>) {
            return Dispatch(node.Children[childIndex] as SyntaxNode <<#LEXER#>,<#OUTPUT#>><#CONTEXT_ARG#>);
    }




 private List<<#OUTPUT#>> VisitManyNonTerminal(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex <#CONTEXT#>)
{
    List<<#OUTPUT#>> results = new List<<#OUTPUT#>>();
    var manyNode = node.Children[childIndex] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>;
    for (int i = 0; i < manyNode.Children.Count; i++)
    {
        results.Add(Dispatch(manyNode.Children[i] as SyntaxNode<<#LEXER#>, <#OUTPUT#>><#CONTEXT_ARG#>));
    }
    return results;
}

private List<Token<<#LEXER#>>> VisitManyTerminal(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex <#CONTEXT#>)
{
    List<Token<<#LEXER#>>> results = new List<Token<<#LEXER#>>>();
    var manyNode = node.Children[childIndex] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>;
    for (int i = 0; i < manyNode.Children.Count; i++)
    {
        results.Add((manyNode.Children[i] as SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>).Token);
    }
    return results;
}

private Group<<#LEXER#>, <#OUTPUT#>> VisitGroup(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex <#CONTEXT#>)
{
    var groupNode = node.Children[childIndex] as SyntaxNode <<#LEXER#>, <#OUTPUT#>>;
    Group <<#LEXER#>, <#OUTPUT#>> group = new Group<<#LEXER#>, <#OUTPUT#>>();
    for (int i = 0; i < groupNode.Children.Count; i++)
    {
        var subNode = groupNode.Children[i];
        if (subNode is SyntaxLeaf <<#LEXER#>, <#OUTPUT#>> leaf && !leaf.Discarded)
        {            
            var token = leaf.Token;
            group.Add(leaf.Name, token);
            continue;
        }
        else if (subNode is SyntaxNode <<#LEXER#>, <#OUTPUT#>> child)
        {
            group.Add(subNode.Name, Dispatch(child<#CONTEXT_ARG#>));
            continue;
        }
    }
    return group;
}


private List<Group<<#LEXER#>, <#OUTPUT#>>> VisitManyGroup(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex <#CONTEXT#>)
{
    List<Group <<#LEXER#>, <#OUTPUT#>>> results = new List<Group<<#LEXER#>, <#OUTPUT#>>>();
    var manyNode = node.Children[childIndex] as SyntaxNode <<#LEXER#>, <#OUTPUT#>>;
    for (int i = 0; i < manyNode.Children.Count; i++)
    {
        var grp = VisitGroup(manyNode, i<#CONTEXT_ARG#>);
        results.Add(grp);
    }
    return results;
}

private ValueOption<Group<<#LEXER#>, <#OUTPUT#>>> VisitOptionalGroup(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex <#CONTEXT#>)
{
    OptionSyntaxNode <<#LEXER#>, <#OUTPUT#>> childNode = node.Children[childIndex] as OptionSyntaxNode<<#LEXER#>, <#OUTPUT#>>;
    if (childNode.Children.Count == 0)
    {
        return new ValueOption<Group<<#LEXER#>, <#OUTPUT#>>>();
    }

    var value = VisitGroup(childNode, 0<#CONTEXT_ARG#>);
    return new ValueOption<Group<<#LEXER#>, <#OUTPUT#>>>(value);
}

private Token<<#LEXER#>> VisitOptionalTerminal(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex <#CONTEXT#>)
{
    var childNode = node.Children[childIndex] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>;
    if (childNode == null || childNode.Children.Count == 0)
    {
        return new Token<<#LEXER#>>()
        {
            IsEmpty = true
        };
    }
    return (childNode.Children[0] as SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>).Token;
}

private ValueOption<<#OUTPUT#>> VisitOptionalNonTerminal(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex <#CONTEXT#>)
{
    OptionSyntaxNode <<#LEXER#>, <#OUTPUT#>> childNode = node.Children[childIndex] as OptionSyntaxNode<<#LEXER#>, <#OUTPUT#>>;
    if (childNode.Children.Count == 0)
    {
        return new ValueOption<<#OUTPUT#>>();
    }

    var value = Dispatch(childNode.Children[0] as SyntaxNode <<#LEXER#>, <#OUTPUT#>><#CONTEXT_ARG#>);
    return new ValueOption<<#OUTPUT#>>(value);
}

public <#OUTPUT#> Visit(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node <#CONTEXT#>)
    {
        var squasher = new Squasher<<#LEXER#>, <#OUTPUT#>>();
        node = squasher.Squash(node);
        node = squasher.FixAssoc(node);
        return Dispatch(node<#CONTEXT_ARG#>);
    }

    private <#OUTPUT#> Dispatch(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node <#CONTEXT#>)
    {
    switch (node.Visitor)
    {
        <#DISPATCHERS#>        
    default:
            throw new Exception($"No visit method defined for rule {node.Visitor}");
    }    
}

    <#VISITORS#>

}