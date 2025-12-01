public <#RETURN_TYPE#> Visit<#NAME#>_<#INDEX#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
    {    
    var arg0 = Visit<#LEFT_NAME#>((SyntaxNode<<#LEXER#>, <#OUTPUT#>>)node.Children[<#INDEX#>]);
        
        if (node.IsByPassNode)
        {
            return arg0;
        }

        var arg1 = (node.Children[1] as SyntaxLeaf<ExprToken, <#OUTPUT#>>).Token;

        <#RETURN#>;




    }