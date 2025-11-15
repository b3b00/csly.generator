using System;
using System.Collections.Generic;
using System.Text;

namespace <#NS#>
{
    internal class <#PARSER#>Main
    {

    public ParseResult<<#OUTPUT#>> Parse(string source)
        {
            Static<#LEXER#> scanner = new Static<#LEXER#>();
    var lexerResult = scanner.Scan(source.AsSpan());

            if (lexerResult.IsError)
            {        
                return new ParseResult(lexerResult.Error);
            }
    var parser = new Static<#PARSER#>();
    var result = parser.ParseNonTerminal_<#ROOT#>(lexerResult.Tokens, 0);
                               if (result.IsOk)
            {
                var visitor = new <#PARSER#>Visitor(parser);
                var output = visitor.Visit<#ROOT#>(result.Node);
                return new ParseResult<<#OUTPUT#>>(output);
            }
            else
            {
                return new ParseResult<<#OUTPUT#>>(result.Error);
            }
}   

    }
}
