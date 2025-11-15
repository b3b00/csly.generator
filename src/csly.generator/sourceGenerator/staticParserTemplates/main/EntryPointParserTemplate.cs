
namespace sourceGenerationTester.visitor;

public class <#PARSER#>Main
    {

    public ParseResult<<#LEXER#>, <#OUTPUT#>> Parse(string source)
        {
            Static<#LEXER#> scanner = new Static<#LEXER#>();
    var lexerResult = scanner.Scan(source.AsSpan());

            if (lexerResult.IsError)
            {        
                return new ParseResult<<#LEXER#>, <#OUTPUT#>>(lexerResult.Error);
            }
    var parser = new Static<#PARSER#>();
    var result = parser.ParseNonTerminal_<#ROOT#>(lexerResult.Tokens, 0);
                               if (result.IsOk)
            {
                var visitor = new <#PARSER#>Visitor(parser);
                var output = visitor.Visit<#ROOT#>(result.Root);
                return new ParseResult<<#OUTPUT#>>(output);
            }
            else
            {
                return new ParseResult<<#OUTPUT#>>(result.Error);
            }
}   

    }

