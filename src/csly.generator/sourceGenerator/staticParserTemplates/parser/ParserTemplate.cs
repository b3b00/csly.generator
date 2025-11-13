
using System;
using System.Collections.Generic;
using System.Linq;
using csly.generator.model.lexer;
using csly.generator.model.parser.tree;
using csly.generator.sourceGenerator;
using csly.generator.model.parser;

namespace <#NAMESPACE#>;

public partial class Static<#PARSER#> : AbstractParserGenerator<<#LEXER#>, <#PARSER#>, <#OUTPUT#>> 
{
    public Dictionary<<#LEXER#>, Dictionary<string, string>> LexemeLabels { get; set; }
    
    public string I18n { get; set; }

    <#HELPERS#>
    
    <#PARSERS#>
}