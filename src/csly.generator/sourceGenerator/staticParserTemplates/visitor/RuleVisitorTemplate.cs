public <#OUTPUT#> Visit<#NAME#>(SyntaxNode<ExpressionToken, int> node)
    {
        <#VISITORS#>
        return _instance.<#VISITOR#>(<#ARGS#>);
    }