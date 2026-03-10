// TODO indentations
if (<#IS_INDENTATION_AWARE#>)
{
    var ind = ConsumeIndents3(source, _currentPosition);

    // move cursor
    _currentPosition = ind.Position;
            
    if (ind != null && ind.IsIndent || ind.IsUnIndent)
    {
        tokens.Add(ind);
    }
}