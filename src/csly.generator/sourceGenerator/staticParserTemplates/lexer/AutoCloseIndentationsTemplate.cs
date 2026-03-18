if (<#IS_INDENTATION_AWARE#> && <#AUTO_CLOSE_INDENTATIONS#>)
{
    int level = 0;
    foreach (var token in tokens)
    {
        if (token.IsIndent)
        {
            level++;
        }
        else if (token.IsUnIndent)
        {
            level--;
        }
    }

    if (level > 0)
    {
        var lastPosition = tokens[tokens.Count-1].Position;
        for (int i = 0; i < level; i++)
        {
            tokens.Add(Token<<#LEXER#>>.UIndent(lastPosition));
        }
    }
            
}