using System.Linq;

namespace <#NAMESPACE#>;

public class <#PARSER#>Main
    {

    private readonly <#PARSER#> _instance;

    public <#PARSER#>Main(<#PARSER#> instance)
        {
        _instance = instance;
        }

    public ParseResult<<#LEXER#>, <#OUTPUT#>> Parse(string source)
        {
            // lexing
            Static<#LEXER#> scanner = new Static<#LEXER#>();
            var lexerResult = scanner.Scan(source.AsSpan());
            
            if (lexerResult.IsError)
            {        
                return new ParseResult<<#LEXER#>, <#OUTPUT#>>(lexerResult.Error);
            }

            // parsing
            var parser = new Static<#PARSER#>();
            var result = parser.ParseNonTerminal_<#ROOT#>(lexerResult.Tokens, 0);
            if (result.IsOk)
            {

                // visiting
                var visitor = new <#PARSER#>Visitor(_instance);
                var output = visitor.Visit<#ROOT#>(result.Root as SyntaxNode<<#LEXER#>, <#OUTPUT#>>);
                return new ParseResult<<#LEXER#>, <#OUTPUT#>>(output, result.Root);
            }
            else
            {
                return new ParseResult<<#LEXER#>, <#OUTPUT#>>(result.Errors.Cast<ParseError>().ToList());
            }
}   

    }

