

// parse terminal <#NAME#>
var r<#INDEX#> = parseTerminalIndent(tokens,position,<#DISCARDED#>);
if (r<#INDEX#>.IsError)
{
    return r<#INDEX#>;
}
position = r<#INDEX#>.EndingPosition;

