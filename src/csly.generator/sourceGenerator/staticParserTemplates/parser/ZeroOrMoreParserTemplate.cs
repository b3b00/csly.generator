
public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseZeroOrMore_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position)
    {
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
    var manyNode = new ManySyntaxNode<<#LEXER#>, <#OUTPUT#>>($"<#INNER_CLAUSE_NAME#>*");
    var currentPosition = position;    
    var stillOk = true;

    SyntaxParseResult <<#LEXER#>, <#OUTPUT#>> lastInnerResult = null;

    var innerErrors = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>();

    bool hasByPasNodes = false;
    while (stillOk)
    {
        SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> innerResult = null;

        <#CALL#>        

        if (innerResult != null && !innerResult.IsError)
        {
            manyNode.Add(innerResult.Root);
            currentPosition = innerResult.EndingPosition;
            lastInnerResult = innerResult;
            hasByPasNodes = hasByPasNodes || innerResult.HasByPassNodes;
            var lastInnerErrors = lastInnerResult.GetErrors();
            if (lastInnerErrors != null)
            {
                innerErrors.AddRange(lastInnerErrors);
            }
        }
        else
        {
            if (innerResult != null)
            {
                innerErrors.AddRange(innerResult.GetErrors());
            }
        }       

        //stillOk = innerResult != null && !innerResult.IsError && currentPosition < tokens.Count;
    }

    result.EndingPosition = currentPosition;
    result.IsError = false;
    result.AddErrors(innerErrors);
    result.Root = manyNode;
    result.IsEnded = lastInnerResult != null && lastInnerResult.IsEnded;
    result.HasByPassNodes = hasByPasNodes;
    return result;
}   