else if (state == LexerStates.InIdentifier){
    if (<#CONDITION#>)
    {
        position.Index++;
        state = LexerStates.InIdentifier;
    }
    else
    {
        var value = source.Slice(previous.Index, position.Index - previous.Index).ToString();
        tokens.Add(new Token<<#LEXER#>>(<#LEXER#>.<#NAME#>, value, previous.Clone()));
        if (currentChar == (char)0)
        {
            position.Index++;
        }
        state = LexerStates.Start;
    }
}