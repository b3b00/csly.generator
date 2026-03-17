 
// parse choice <#NAME#>
var r<#INDEX#> = ParseChoice_<#NAME#>(tokens, currentPosition, parsingContext);
innerResult = r<#INDEX#>;
stillOk = innerResult != null && !innerResult.IsError && currentPosition < tokens.Count;
if (stillOk)
{
    currentPosition = innerResult.EndingPosition;
    manyNode.IsManyValues = !<#IS_GROUP#>;
    manyNode.IsManyGroups = <#IS_GROUP#> ;
    manyNode.IsManyTokens = false;
    //manyNode.Add(innerResult.Root);
}

