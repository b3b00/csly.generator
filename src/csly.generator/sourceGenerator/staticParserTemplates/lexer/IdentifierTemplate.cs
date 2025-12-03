if (state == LexerStates.InIdentifier){
    if (<#CONDITION#>)
    {
        position.Index++;
        state = LexerStates.InIdentifier;
    }
    else
    {
        var value = source.Slice(previous.Index, position.Index - previous.Index).ToString();
        // TODO : Check for keywords here
        if (_keyWords.TryGetValue(value, out var keyword)) {
            tokens.Add(new Token<<#LEXER#>>(keyword, value, previous.Clone()));
        }
        else if (_explicitKeywords.Contains(match.Value.ToString()))
        {
            return new Token<ExprToken>(ExprToken.ID, match.Value, match.Position)
            {
                IsExplicit = true
            };
        }
        else
        {
            tokens.Add(new Token<<#LEXER#>>(<#LEXER#>.<#NAME#>, value, previous.Clone()));
        if (currentChar == (char)0)
            {
                position.Index++;
            }
        }
        state = LexerStates.Start;
    }
}