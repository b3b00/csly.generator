 
// parse non terminal <#NAME#>
var r<#INDEX#> = ParseMany_<#NAME#>(tokens,position, parsingContext);
 if (r<#INDEX#>.IsError && <#NOT_EMPTY#>)
 {
     return r<#INDEX#>;
 }
 position = r<#INDEX#>.EndingPosition;
 
