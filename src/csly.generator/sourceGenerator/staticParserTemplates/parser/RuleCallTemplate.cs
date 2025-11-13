var r<#INDEX#>Leadings = new LeadingToken<<#LEXER#>>[]
        {
            <#LEADINGS#>
        };
if (r<#INDEX#>Leadings.Any(x => x.Match(token))) {
            var r<#INDEX#> = ParseRule_<#NAME#>_<#INDEX#>(tokens, position);
            if (r<#INDEX#>.IsOk)
            {
    return r<#INDEX#>;
            }
results.Add(r<#INDEX#>);
        }