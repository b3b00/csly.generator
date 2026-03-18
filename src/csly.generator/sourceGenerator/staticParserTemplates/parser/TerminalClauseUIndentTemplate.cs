

// parse terminal <#NAME#>
var r<#INDEX#> = parseTerminalUIndent(tokens,position,<#DISCARDED#>);
if (r<#INDEX#>.IsError)
{
    return r<#INDEX#>;
}
position = r<#INDEX#>.EndingPosition;

