private <#OUTPUT#> Visit<#NAME#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
    var arg0 = Dispatch(node.Children[0] as SyntaxNode<<#LEXER#>, <#OUTPUT#>>);
    if (node.Children.Count == 1)
    {
        return arg0;
    }

    Token<<#LEXER#>> arg1 = (node.Children[1] as SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>).Token;

    <#RETURN#>
}