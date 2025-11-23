

// parse terminal <#NAME#>
var r<#INDEX#> = ParseTerminal_<#NAME#>(tokens, currentPosition);
<#INDEX#>Result = rinner;
stillOk = innerResult != null && !innerResult.IsError && currentPosition < tokens.Count;
if (stillOk)
{   
    currentPosition = innerResult.EndingPosition;
    manyNode.IsManyValues = false;
    manyNode.IsManyGroups = false;
    manyNode.IsManyTokens = true;    
//    manyNode.Add(innerResult.Root);
}

