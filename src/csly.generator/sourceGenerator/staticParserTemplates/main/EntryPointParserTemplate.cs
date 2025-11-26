using System.Linq;

namespace <#NAMESPACE#>;

public class <#PARSER#>Main
    {

    private readonly <#PARSER#> _instance;

    public <#LEXER#>_FsmLexer Lexer => new <#LEXER#>_FsmLexer();

    public <#PARSER#>Main(<#PARSER#> instance)
        {
        _instance = instance;
        }

    public ParseResult<<#LEXER#>, <#OUTPUT#>> Parse(string source)
        {
            // lexing
            <#LEXER#>_FsmLexer scanner = new <#LEXER#>_FsmLexer();
            var lexerResult = scanner.Scan(source.AsSpan());
            
            if (lexerResult.IsError)
            {        
                return new ParseResult<<#LEXER#>, <#OUTPUT#>>(lexerResult.Error);
            }
else {
    int count = lexerResult.Tokens.Count;
    Console.WriteLine($"Lexing successful. {lexerResult.Tokens.Count} Tokens:");
    for (int i = 0; i < count; i++)
    {
        var token = lexerResult.Tokens[i];
        Console.WriteLine($"#{i}: {token.ToString()}");
    }
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

