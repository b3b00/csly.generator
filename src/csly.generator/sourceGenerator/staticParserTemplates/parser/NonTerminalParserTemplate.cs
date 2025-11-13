public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> ParseNonTerminal_<#NAME#>(List<Token<<#LEXER#>>> tokens, int position)
    {
        var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
        var token = tokens[position];
        var results = new List<SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>>();

        <#CALLS#>

        result.IsError = true;
        result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : new List<UnexpectedTokenSyntaxError<<#LEXER#>>>()).ToList());
        return result;
    }
        