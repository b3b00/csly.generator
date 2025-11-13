
if (currentChar == '<#CHAR#>')
{
    tokens.Add(new Token<<#LEXER#>>(ExpressionToken.<#NAME#>, "<#CHAR#>", position));
    position.Index++;
    previous = position.Clone();
}