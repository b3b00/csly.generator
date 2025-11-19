
if (<#LEXEME_CONDITION#>) // Sugar : "<#NAME#>"
{
    tokens.Add(new Token<<#LEXER#>>(<#LEXER#>.<#NAME#>, "<#PATTERN#>", position));
    position.Index++;
    previous = position.Clone();
    state = LexerStates.<#NEW_STATE#>;
}