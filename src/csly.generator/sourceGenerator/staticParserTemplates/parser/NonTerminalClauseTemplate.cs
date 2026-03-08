 
// parse non terminal <#NAME#>
var r<#INDEX#> = ParseNonTerminal_<#NAME#>(tokens,position, parsingContext);
 if (r<#INDEX#>.IsError)
 {
     return r<#INDEX#>;
 }
 position = r<#INDEX#>.EndingPosition;
 
