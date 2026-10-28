private <#OUTPUT#> Visit<#NAME#>_<#INDEX#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node <#CONTEXT#>)
{
    var arg0 = Dispatch(node.Children[0] as SyntaxNode<<#LEXER#>, <#OUTPUT#>><#CONTEXT_ARG#>);
    if (node.Children.Count == 1)
    {
        return arg0;
    }

    Token<<#LEXER#>> arg1 = (node.Children[1] as SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>).Token;

    <#RETURN#>
}