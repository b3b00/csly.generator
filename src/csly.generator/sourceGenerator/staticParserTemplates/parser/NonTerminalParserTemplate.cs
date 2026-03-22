
public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseNonTerminal_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position, ParsingContext<<#LEXER#>, <#OUTPUT#>> parsingContext)
    {

    if (parsingContext.TryGetParseResult("<#NAME#>", position, out var cachedResult))
    {
        return cachedResult;
    }
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>(); 
        var token = tokens[position];
        var results = new List<SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>>(<#RULES_COUNT#>);

        var expectedTokens = new List<LeadingToken<<#LEXER#>>>(<#EXPECTED_COUNT#>) { <#EXPECTEDTOKENS#> };

        <#CALLS#>
        
        var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
        if (okResult != null && okResult.IsOk)
        {
        parsingContext.Memoize("<#NAME#>", position, okResult);
        return okResult;
        }

    result.IsError = true;
    var allExpected = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>() { new UnexpectedTokenSyntaxError<<#LEXER#>>(tokens[position],"en", expectedTokens) };


    var accumulatedErrors = AccumulateErrors(results, <#RULES_COUNT#>);
    result.AddErrors(accumulatedErrors);
    parsingContext.Memoize("<#NAME#>", position, result);
    return result;
    }
        