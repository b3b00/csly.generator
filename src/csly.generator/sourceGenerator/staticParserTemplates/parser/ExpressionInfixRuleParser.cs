
///////////////////////////////////////
// INFIX RULE <#RULESTRING#>
///////////////////////////////////////

public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseRule_<#HEAD#>_<#INDEX#>(List<Token<<#LEXER#>>> tokens, int position)
{
    var result = new SyntaxParseResult<<#LEXER#>, string>();


    // parse non terminal Expr_Prec_50 
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
        return r0;
    }
    position = r1.EndingPosition;

    // parse non terminal Expr_Prec_10
    var r2 = ParseNonTerminal_<#HEAD#>(tokens, position); 
    if (r2.IsError)
    {
        return r2;
    }
    position = r2.EndingPosition;




    // TODO node name
    var tree = new SyntaxNode<<#LEXER#>, string>("<#HEAD#>", new List<ISyntaxNode<<#LEXER#>, string>>() { r0.Root, r1.Root, r2.Root },
        "Expr_Prec_10_0");
    tree.ExpressionAffix = Affix.<#AFFIX#>;
    tree.Precedence = <#PRECEDENCE#>;
    tree.Associativity = Associativity.<#ASSOCIATIVITY#>;
    tree.IsExpressionNode = true;
    result.Root = tree;
    result.IsError = false;
    result.EndingPosition = r2.EndingPosition;

    return result;
}

