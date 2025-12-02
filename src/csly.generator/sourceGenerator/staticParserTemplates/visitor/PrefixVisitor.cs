private <#OUTPUT#> Visit<#NAME#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
    if (node.Children.Count == 2)
    {
        Token<<#LEXER#>> arg0 = (node.Children[0] as SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>).Token;
        var arg1 = Dispatch(node.Children[1] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>);

       <#RETURN#>
    }
    return Dispatch(node.Children[1] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>);
}