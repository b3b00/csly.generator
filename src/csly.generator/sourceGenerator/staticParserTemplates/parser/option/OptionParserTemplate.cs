
public SyntaxParseResult<<#LEXER#>,<#OUTPUT#>> ParseOption_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position, ParsingContext<<#LEXER#>, <#OUTPUT#>> parsingContext)
{
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
    var optionNode = new OptionSyntaxNode<<#LEXER#>, <#OUTPUT#>>($"<#INNER_CLAUSE_NAME#>?");
    var currentPosition = position;

    SyntaxParseResult <<#LEXER#>, <#OUTPUT#>> innerResult = null;
    <#CALL#>

    var innerErrors = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>();

    bool hasByPasNodes = false;
    
        

    <#CALL#>        

    if (innerResult != null && !innerResult.IsError)
    {
        optionNode.Children.Add(innerResult.Root);
        currentPosition = innerResult.EndingPosition;        
        hasByPasNodes = hasByPasNodes || innerResult.HasByPassNodes;        
    }

    result.EndingPosition = currentPosition;
    result.IsError = false;
    result.AddErrors(innerErrors);
    result.Root = optionNode;
    result.IsEnded = innerResult != null && innerResult.IsEnded;
    result.HasByPassNodes = hasByPasNodes;
    

    return result;
}   