public <#OUTPUT#> Visit<#NAME#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
    switch (node.Visitor)
    {
        <#VISITORS#>
        default:
                throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
    }
            return 0;
}