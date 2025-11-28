
///////////////////////////////////////
// RULE <#RULESTRING#>
///////////////////////////////////////

///////////////////////////////////////
// RULE <#NAME#>
///////////////////////////////////////
public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseRule_<#HEAD#>_<#INDEX#>(List<Token<<#LEXER#>>> tokens, int position)
{
    var result = new SyntaxParseResult<Toky, string>();


    // parse non terminal Expr_Prec_50 
    var r0 = ParseNonTerminal_<#LOWER_PRECEDENCE#>(tokens, position); // TODO
    if (r0.IsError)
    {
        return r0;
    }
        
    var r1 = ParseChoice_<#OPERATOR#>(tokens, position);  // TODO
    position = r1.EndingPosition;

    if (r1.IsError)
    {
        return r0;
    }

    // parse non terminal Expr_Prec_10
    var r2 = ParseNonTerminal_<#HEAD#>(tokens, position); // TODO
    if (r2.IsError)
    {
        return r2;
    }
    position = r2.EndingPosition;




    // TODO node name
    var tree = new SyntaxNode<Toky, string>("<#HEAD#>", new List<ISyntaxNode<Toky, string>>() { r0.Root, r1.Root, r2.Root },
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



public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseRule_<#HEAD#>_<#INDEX#>(List<Token<<#LEXER#>>> tokens, int position)
{
     var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
     
     <#CLAUSES#>
     
     var tree = new SyntaxNode<<#LEXER#>, <#OUTPUT#>>("<#HEAD#>", new List<ISyntaxNode<<#LEXER#>, <#OUTPUT#>>>() { <#CHILDREN#> },
         "<#NAME#>_<#INDEX#>");
     result.Root = tree;
     result.IsError = false;
     result.EndingPosition = r<#RULE_COUNT#>.EndingPosition;
     return result;
}