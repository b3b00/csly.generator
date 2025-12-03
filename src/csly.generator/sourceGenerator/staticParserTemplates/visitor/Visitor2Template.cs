
using csly.models;

namespace <#NAMESPACE#>;

internal class <#PARSER#>Visitor2
{
    private readonly <#PARSER#> _instance;

    public <#PARSER#>Visitor2(<#PARSER#> instance)    {
        _instance = instance;
    }

private Token<<#LEXER#>> VisitTerminal(SyntaxNode<<#LEXER#>,<#OUTPUT#>> node, int childIndex) {
            return (node.Children[childIndex] as SyntaxLeaf<<#LEXER#>,<#OUTPUT#>>).Token;
    }

private <#OUTPUT#> VisitNonTerminal(SyntaxNode<<#LEXER#>,<#OUTPUT#>> node, int childIndex) {
            return Dispatch(node.Children[childIndex] as SyntaxNode <<#LEXER#>,<#OUTPUT#>>);
    }

 private List<<#OUTPUT#>> VisitManyNonTerminal(SyntaxNode<ExprToken, int> node, int childIndex)
{
    List<int> results = new List<int>();
    var manyNode = node.Children[childIndex] as SyntaxNode<ExprToken, int>;
    for (int i = 0; i < manyNode.Children.Count; i++)
    {
        results.Add(Dispatch(manyNode.Children[i] as SyntaxNode<ExprToken, int>));
    }
    return results;
}

private List<Token<<#LEXER#>>> VisitManyTerminal(SyntaxNode<ExprToken, int> node, int childIndex)
{
    List<Token<ExprToken>> results = new List<Token<ExprToken>>();
    var manyNode = node.Children[childIndex] as SyntaxNode<ExprToken, int>;
    for (int i = 0; i < manyNode.Children.Count; i++)
    {
        results.Add((manyNode.Children[i] as SyntaxLeaf<ExprToken, int>).Token);
    }
    return results;
}

private Token<ExprToken> VisitOptionalTerminal(SyntaxNode<ExprToken, int> node, int childIndex)
{
    var childNode = node.Children[childIndex] as SyntaxNode<ExprToken, int>;
    if (childNode == null || childNode.Children.Count == 0)
    {
        return new Token<ExprToken>()
        {
            IsEmpty = true
        };
    }
    return (childNode.Children[0] as SyntaxLeaf<ExprToken, int>).Token;
}

private ValueOption<<#OUTPUT#>> VisitOptionalNonTerminal(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node, int childIndex)
{
    OptionSyntaxNode <<#LEXER#>, <#OUTPUT#>> childNode = node.Children[childIndex] as OptionSyntaxNode<<#LEXER#>, <#OUTPUT#>>;
    if (childNode.Children.Count == 0)
    {
        return new ValueOption<<#OUTPUT#>>();
    }

    var value = Dispatch(childNode.Children[0] as SyntaxNode <<#LEXER#>, <#OUTPUT#>>);
    return new ValueOption<<#OUTPUT#>>(value);
}

public <#OUTPUT#> Visit(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
    {
        var squasher = new Squasher<<#LEXER#>, <#OUTPUT#>>();
        node = squasher.Squash(node);
        node = squasher.FixAssoc(node);
        return Dispatch(node);
    }

    private <#OUTPUT#> Dispatch(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
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