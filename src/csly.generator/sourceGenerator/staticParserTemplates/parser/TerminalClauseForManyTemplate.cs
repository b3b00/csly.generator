

// parse terminal <#NAME#>
var r<#INDEX#> = ParseTerminal_<#NAME#>(tokens, currentPosition);
innerResult = r<#INDEX#>;
stillOk = innerResult != null && !innerResult.IsError && currentPosition < tokens.Count;
if (stillOk)
{   
    currentPosition = innerResult.EndingPosition;
    manyNode.IsManyValues = false;
    manyNode.IsManyGroups = false;
    manyNode.IsManyTokens = true;
}
