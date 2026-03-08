 
// parse choice <#NAME#>
var r<#INDEX#> = ParseChoice_<#NAME#>(tokens,position, parsingContext); 
position = r<#INDEX#>.EndingPosition;
if (r<#INDEX#>.IsError)
{
    return r<#INDEX#>; // new
}
position = r<#INDEX#>.EndingPosition; 
