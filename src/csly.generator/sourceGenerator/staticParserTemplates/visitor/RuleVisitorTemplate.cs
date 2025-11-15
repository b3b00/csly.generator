public <#OUTPUT#> Visit<#NAME#>_<#INDEX#>(SyntaxNode<ExpressionToken, int> node)
    {
        <#VISITORS#>
        return _instance.<#VISITOR#>(<#ARGS#>);
    }