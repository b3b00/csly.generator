

// parse terminal <#NAME#>
var r<#INDEX#> = ParseTerminal_<#NAME#>(tokens,position);
if (r<#INDEX#>.IsError)
{
    return r<#INDEX#>;
}
position = r<#INDEX#>.EndingPosition;

