
///////////////////////////////////////
// INFIX RULE <#RULESTRING#>
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

    var r1 = ParseChoice_<#OPERATOR#>(tokens, position);  
    position = r1.EndingPosition;

    if (r1.IsError)
    {

        // TODO node name
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

    var r2 = ParseNonTerminal_<#HEAD#>(tokens, position); 
    if (r2.IsError)
    {
        return r2;
    }
    position = r2.EndingPosition;


    var tree = new SyntaxNode<<#LEXER#>, <#OUTPUT#>>("<#HEAD#>", new List<ISyntaxNode<<#LEXER#>, <#OUTPUT#>>>() { r0.Root, r1.Root, r2.Root },
        "<#HEAD#>_<#INDEX#>");
    tree.ExpressionAffix = Affix.<#AFFIX#>;
    tree.Precedence = <#PRECEDENCE#>;
    tree.Associativity = Associativity.<#ASSOCIATIVITY#>;
    tree.IsExpressionNode = true;
    result.Root = tree;
    result.IsError = false;
    result.EndingPosition = r2.EndingPosition;

    return result;
}

