private <#OUTPUT#> Visit<#NAME#>_<#INDEX#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
    var arg0 = VisitNonTerminal(node, 0);
    if (node.IsByPassNode)
    {
        return arg0;
    }

    Token<<#LEXER#>> arg1 = VisitTerminal(node,1);
    var arg2 = VisitNonTerminal(node, 2);



<#RETURN#>

}