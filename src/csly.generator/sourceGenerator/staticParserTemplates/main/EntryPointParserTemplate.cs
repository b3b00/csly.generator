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


// parsing
var parser = new Static<#PARSER#>();
            var result = parser.ParseNonTerminal_<#ROOT#>(lexerResult.Tokens, 0);
            if (result.IsOk)
            {
                Console.WriteLine("Parsing succeeded.");
                //Console.WriteLine($"Parse Tree: {result.Root.Dump("  ")}");
                
                // visiting
                var visitor = new <#PARSER#>Visitor2(_instance);
                var output = visitor.Visit(result.Root as SyntaxNode<<#LEXER#>, <#OUTPUT#>>);
                return new ParseResult<<#LEXER#>, <#OUTPUT#>>(output, result.Root);
            }
            else
            {
                return new ParseResult<<#LEXER#>, <#OUTPUT#>>(result.Errors.Cast<ParseError>().ToList());
            }
}   

    }

