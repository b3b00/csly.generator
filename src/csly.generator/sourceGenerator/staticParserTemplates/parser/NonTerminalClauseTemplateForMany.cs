 
// parse non terminal <#NAME#>
var r<#INDEX#> = ParseNonTerminal_<#NAME#>(tokens, currentPosition);
innerResult = rinner;
stillOk = innerResult != null && !innerResult.IsError && currentPosition < tokens.Count;
if (stillOk)
{
    currentPosition = innerResult.EndingPosition;
    manyNode.IsManyValues = { !nonTerminalClause.IsGroup};
    manyNode.IsManyGroups = { nonTerminalClause.IsGroup};
    manyNode.IsManyTokens = false;
    //manyNode.Add(innerResult.Root);
}

