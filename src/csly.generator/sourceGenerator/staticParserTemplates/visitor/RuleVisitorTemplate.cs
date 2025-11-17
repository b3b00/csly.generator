public <#OUTPUT#> Visit<#NAME#>_<#INDEX#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
    {
        <#VISITORS#>
        return _instance.<#VISITOR#>(<#ARGS#>);
    }