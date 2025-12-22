
public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseNonTerminal_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position)
    {
    Console.WriteLine("Parsing <#NAME#> at position " + position);
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
        var token = tokens[position];
        var results = new List<SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>>();

        var expectedTokens = new List<LeadingToken<<#LEXER#>>>() { <#EXPECTEDTOKENS#> };

        <#CALLS#>
        
        var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
        if (okResult != null && okResult.IsOk)
        {
            return okResult;
        }

    result.IsError = true;
    var allExpected = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>() { new UnexpectedTokenSyntaxError<<#LEXER#>>(tokens[position],"en", expectedTokens) };


    result.AddErrors(results.SelectMany(x => x.Errors != null && x.Errors.Count > 0 ? x.GetErrors() : new List<UnexpectedTokenSyntaxError<<#LEXER#>>>()).ToList());
    result.AddErrors(allExpected);
        return result;
    }
        