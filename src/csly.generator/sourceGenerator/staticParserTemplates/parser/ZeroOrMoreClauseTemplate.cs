 
// parse non terminal <#NAME#>
var r<#INDEX#> = ParseZeroOrMore_<#NAME#>(tokens,position);
 if (r<#INDEX#>.IsError)
 {
     return r<#INDEX#>;
 }
 position = r<#INDEX#>.EndingPosition;
 
