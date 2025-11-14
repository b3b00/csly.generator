
using System;
using System.Collections.Generic;
using System.Linq;
using csly.models;

namespace <#NAMESPACE#>;

public partial class Static<#PARSER#> : AbstractParserGenerator<<#LEXER#>, <#PARSER#>, <#OUTPUT#>> 
{
    public Dictionary<<#LEXER#>, Dictionary<string, string>> LexemeLabels { get; set; }
    
    public string I18n { get; set; }

    <#HELPERS#>
    
    <#PARSERS#>
}