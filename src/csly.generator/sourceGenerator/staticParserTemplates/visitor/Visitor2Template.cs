
using csly.models;

namespace <#NAMESPACE#>;

internal class <#PARSER#>Visitor2
{
    private readonly <#PARSER#> _instance;

    public <#PARSER#>Visitor2(<#PARSER#> instance)    {
        _instance = instance;
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