
public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseChoice_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position)
    {
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
        var token = tokens[position];

    <#CHOICECALLLIST#>        

    var results = new List<SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>>();

        var expectedTokens = new List<LeadingToken<<#LEXER#>>>() { <#EXPECTEDTOKENS#> };

        result.IsError = true;
    var allExpected = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>() { new UnexpectedTokenSyntaxError<<#LEXER#>>(tokens[position],"en", expectedTokens) };
    result.AddErrors(results.SelectMany(x => x.Errors != null && x.Errors.Count > 0? x.GetErrors() : allExpected).ToList());
    return result;
}
