
///////////////////////////////////////
// POSTFIX EXPRESSION <#RULESTRING#> TODO !
///////////////////////////////////////
public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseRule_<#HEAD#>_<#INDEX#>(List<Token<<#LEXER#>>> tokens, int position)
{
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();

    var r0 = ParseNonTerminal_<#LOWER_PRECEDENCE#>(tokens, position);
    if (r0.IsError)
    {
        return r0;
    }
    position = r0.EndingPosition;

    var r1 = <#TOKEN_KIND#>_<#OPERATOR#>(tokens, position);  
    position = r1.EndingPosition;

    if (r1.IsError)
    {                
        var leftTree = new SyntaxNode<<#LEXER#>, <#OUTPUT#>>("<#HEAD#>", new List<ISyntaxNode<<#LEXER#>, <#OUTPUT#>>>() { r0.Root },
        "<#HEAD#>_<#INDEX#>");
        leftTree.ExpressionAffix = Affix.<#AFFIX#>;
        leftTree.Precedence = <#PRECEDENCE#>;
        leftTree.Associativity = Associativity.<#ASSOCIATIVITY#>;
        leftTree.IsExpressionNode = true;
        leftTree.IsByPassNode = true;
        result.Root = leftTree;
        result.IsError = false;
        result.EndingPosition = r0.EndingPosition;

        return result;

    }
    position = r1.EndingPosition;

   


    var tree = new SyntaxNode<<#LEXER#>, <#OUTPUT#>>("<#HEAD#>", new List<ISyntaxNode<<#LEXER#>, <#OUTPUT#>>>() { r0.Root, r1.Root},
        "<#HEAD#>_<#INDEX#>");
    tree.ExpressionAffix = Affix.<#AFFIX#>;
    tree.Precedence = <#PRECEDENCE#>;
    tree.Associativity = Associativity.<#ASSOCIATIVITY#>;
    tree.IsExpressionNode = true;
    result.Root = tree;
    result.IsError = false;
    result.EndingPosition = r1.EndingPosition;

    return result;
}