
if (<#LEXEME_CONDITION#>)
{
    state = LexerStates.<#NEW_STATE#>;
    previous = position.Clone();    
    position.Index++;
}