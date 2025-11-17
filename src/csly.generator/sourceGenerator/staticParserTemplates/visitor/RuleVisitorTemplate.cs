public <#OUTPUT#> Visit<#NAME#>_<#INDEX#>(SyntaxNode<ExpressionToken, <#OUTPUT#>> node)
    {
        <#VISITORS#>
        return _instance.<#VISITOR#>(<#ARGS#>);
    }