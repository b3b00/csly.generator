
#region helpers


private static bool AnyLeadingMatches(LeadingToken<<#LEXER#>>[] leadings, Token<<#LEXER#>> token)
{
    for (int i = 0; i < leadings.Length; i++)
    {
        if (leadings[i].Match(token))
        {
            return true;
        }
    }
    return false;
}

private static List<UnexpectedTokenSyntaxError<<#LEXER#>>> AccumulateErrors(List<SyntaxParseResult<<#LEXER#>, string>> results, int estimatedCapacity = 4)
{
    var accumulatedErrors = new List<UnexpectedTokenSyntaxError<<#LEXER#>>>(estimatedCapacity);
    for (int i = 0; i < results.Count; i++)
    {
        if (results[i].Errors != null && results[i].Errors.Count > 0)
        {
            var errors = results[i].GetErrors();
            for (int j = 0; j < errors.Count; j++)
            {
                accumulatedErrors.Add(errors[j]);
            }
        }
    }
    return accumulatedErrors;
}

private static SyntaxParseResult<<#LEXER#>, string> FindBestOkResult(List<SyntaxParseResult<<#LEXER#>, string>> results)
{
    SyntaxParseResult<<#LEXER#>, string> best = null;
    int maxPosition = -1;
    for (int i = 0; i < results.Count; i++)
    {
        if (results[i].IsOk && results[i].EndingPosition > maxPosition)
        {
            maxPosition = results[i].EndingPosition;
            best = results[i];
        }
    }
    return best;
}


public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> parseTerminal(List<Token<<#LEXER#>>> tokens, <#LEXER#> expected, int position,
        bool discarded = false)
    {
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();
        var token = tokens[position];
    result.IsError = token.IsEOS || expected != token.TokenID;
    result.EndingPosition = !result.IsError ? position + 1 : position;

    if (result.IsError)
    {
        result.AddError(new UnexpectedTokenSyntaxError<<#LEXER#>>(token, LexemeLabels, I18n, new LeadingToken<<#LEXER#>>(expected)));
        }
    else
    {

        token.Discarded = discarded;
        token.IsExplicit = false;
        result.Root = new SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>(token, discarded);
            result.HasByPassNodes = false;
    }

    return result;
}

public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> parseExplicitTerminal(List<Token<<#LEXER#>>> tokens, string expected, int position,
        bool discarded = false)
    {
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();

        result.EndingPosition = !result.IsError ? position + 1 : position;

    var leading = new LeadingToken<<#LEXER#>>(expected);

        result.IsError = !leading.Match(tokens[position]);
    var token = tokens[position];

    if (result.IsOk)
    {
        token.Discarded = discarded;
        token.IsExplicit = false;
        result.Root = new SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>(token, discarded);
            result.HasByPassNodes = false;
    }
    else
    {
        result.AddError(new UnexpectedTokenSyntaxError<<#LEXER#>>(token, LexemeLabels, I18n, leading));
        }

    return result;
}

public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> parseTerminalIndent(List<Token<<#LEXER#>>> tokens, int position, bool discarded = false) 
{
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();

    result.EndingPosition = !result.IsError ? position + 1 : position;
        
    var leading = new LeadingToken<<#LEXER#>>(true, false);

    var token = tokens[position];
    result.IsError = !token.IsIndent;
    
    if (result.IsOk)
    {
        token.Discarded = discarded;
        token.IsExplicit = false;
        result.Root = new SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>(token, discarded);
        result.HasByPassNodes = false;
    }
    else
    {
        result.AddError(new UnexpectedTokenSyntaxError<<#LEXER#>>(token, LexemeLabels, I18n, leading));
    }

    return result;
}

public SyntaxParseResult<<#LEXER#>, <#OUTPUT#>> parseTerminalUIndent(List<Token<<#LEXER#>>> tokens, int position, bool discarded = false) 
{
    var result = new SyntaxParseResult<<#LEXER#>, <#OUTPUT#>>();

    result.EndingPosition = !result.IsError ? position + 1 : position;
        
    var leading = new LeadingToken<<#LEXER#>>(false,true);

    var token = tokens[position];
    result.IsError = !token.IsUnIndent;
    
    if (result.IsOk)
    {
        token.Discarded = discarded;
        token.IsExplicit = false;
        result.Root = new SyntaxLeaf<<#LEXER#>, <#OUTPUT#>>(token, discarded);
        result.HasByPassNodes = false;
    }
    else
    {
        result.AddError(new UnexpectedTokenSyntaxError<<#LEXER#>>(token, LexemeLabels, I18n, leading));
    }

    return result;
}

#endregion
