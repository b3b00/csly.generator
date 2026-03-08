


public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseChoice_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position, ParsingContext<<#LEXER#>, <#OUTPUT#>> parsingContext)
    {
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
        var token = tokens[position];

    var results = new List<SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>>();

    <#CHOICECALLLIST#>        

    

        var expectedTokens = new List<LeadingToken<<#LEXER#>>>() { <#EXPECTEDTOKENS#> };

        result.IsError = true;
    var allExpected = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>() { new UnexpectedTokenSyntaxError<<#LEXER#>>(tokens[position],"en", expectedTokens) };
    var accumulatedErrors = results.SelectMany(x => x.Errors != null && x.Errors.Count > 0? x.GetErrors().ToList() : new List<UnexpectedTokenSyntaxError<<#LEXER#>>>()).ToList();
    //result.AddErrors(results.SelectMany(x => x.Errors != null && x.Errors.Count > 0? x.GetErrors() : allExpected).ToList());
    allExpected.AddRange(accumulatedErrors);
    result.AddErrors(allExpected);
    return result;
}
