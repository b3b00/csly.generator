else if (state == LexerStates.InInt)
{
    if (char.IsDigit(currentChar))
    {
        position.Index++;
        state = LexerStates.InInt;
    }
    else
    {
        var intValue = source.Slice(previous.Index, position.Index - previous.Index).ToString();
        tokens.Add(new Token<<#LEXER#>>(<#LEXER#>.<#NAME#>, intValue, previous.Clone()));
        if (currentChar == (char)0)
        {
            position.Index++;
        }
        state = LexerStates.Start;
    }
}