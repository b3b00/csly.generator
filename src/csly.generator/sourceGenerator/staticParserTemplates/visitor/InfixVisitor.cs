private <#OUTPUT#> Visit<#NAME#>_<#INDEX#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node <#CONTEXT#>)
{
    var arg0 = VisitNonTerminal(node, 0<#CONTEXT_ARG#>);
    if (node.IsByPassNode)
    {
        return arg0;
    }

    Token<<#LEXER#>> arg1 = VisitTerminal(node,1<#CONTEXT_ARG#>);
    var arg2 = VisitNonTerminal(node, 2<#CONTEXT_ARG#>);



<#RETURN#>

}