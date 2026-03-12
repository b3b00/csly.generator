var r<#INDEX#>Leadings = new LeadingToken<<#LEXER#>>[]
        {
            <#LEADINGS#>
        };
if (<#MAY_BE_EMPTY#> || r<#INDEX#>Leadings.Any(x => x.Match(token))) {
    var r<#INDEX#> = ParseRule_<#NAME#>_<#INDEX#>(tokens, position, parsingContext);
    results.Add(r<#INDEX#>);
}