case "<#NAME#>": {
    var result<#NAME#> = parser.ParseNonTerminal_<#NAME#>(mainTokens, 0, new ParsingContext <<#LEXER#>, <#OUTPUT#>>(_useMemoization));
    if (result<#NAME#>.IsOk)
    {
        if (result<#NAME#>.EndingPosition < mainTokens.Count - 1)
        {
            ParseResult <<#LEXER#>, <#OUTPUT#>> notEndedResult = new ParseResult <<#LEXER#>, <#OUTPUT#>>(result<#NAME#>.GetCompactedParseErrors());
            return notEndedResult;
        }
        // visiting
        var visitor = new <#PARSER#>Visitor2(_instance);
        var output = visitor.Visit(result<#NAME#>.Root as SyntaxNode <<#LEXER#>, <#OUTPUT#>>);
        return new ParseResult <<#LEXER#>, <#OUTPUT#>>(output, result<#NAME#>.Root);
    }
    else
    {
        return new ParseResult<<#LEXER#>, <#OUTPUT#>>(result<#NAME#>.GetParseErrors());
    }
}