private <#OUTPUT#> Visit<#NAME#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
    var arg0 = Dispatch(node.Children[0] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>);
    if (node.IsByPassNode)
    {
        return arg0;
    }

    Token<<#LEXER#>> arg1 = (node.Children[1] as SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>).Token;
    var arg2 = Dispatch(node.Children[2] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>);



<#RETURN#>

}