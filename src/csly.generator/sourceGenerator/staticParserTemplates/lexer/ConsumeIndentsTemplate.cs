// TODO indentations
if (<#IS_INDENTATION_AWARE#>)
{
    var ind = ConsumeIndents3(source, _currentPosition);
    if (ind.IsIndentationError)
    {
        var error = new IndentationError(ind.Position, "en"); // TODO i18n
        return (error,ind.Position,false,null);
    }
    // move cursor
    _currentPosition = ind.Position;
            
    if (ind != null && ind.IsIndent || ind.IsUnIndent)
    {
        AddToken(ind);
    }
}