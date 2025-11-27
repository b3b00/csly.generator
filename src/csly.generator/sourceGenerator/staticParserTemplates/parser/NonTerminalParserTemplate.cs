
public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseNonTerminal_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position)
    {
        var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
        var token = tokens[position];
        var results = new List<SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>>();

        var expectedTokens = new List<LeadingToken<<#LEXER#>>>() { <#EXPECTEDTOKENS#> };

        <#CALLS#>

        result = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
        if (result != null && result.IsOk)
        {
            return result;
        }

    result.IsError = true;
    var allExpected = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>() { new UnexpectedTokenSyntaxError<<#LEXER#>>(tokens[position],"en", expectedTokens) };
    result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected ).ToList());
        return result;
    }
        