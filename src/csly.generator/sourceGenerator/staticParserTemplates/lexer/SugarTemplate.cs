
if (currentChar == '<#CHAR#>')
{
    tokens.Add(new Token<<#LEXER#>>(<#LEXER#>.<#NAME#>, "<#CHAR#>", position));
    position.Index++;
    previous = position.Clone();
}