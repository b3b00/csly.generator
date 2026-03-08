using System.Linq;

namespace <#NAMESPACE#>;

public class <#PARSER#>Main
    {

    private readonly <#PARSER#> _instance;

    public <#LEXER#>_MainLexer Lexer => new <#LEXER#>_MainLexer();

    private readonly bool _useMemoization = false;

    public <#PARSER#>Main(<#PARSER#> instance, bool useMemoization = false)
    {
        _instance = instance;
        _useMemoization = useMemoization;
    }

    public ParseResult<<#LEXER#>, <#OUTPUT#>> Parse(string source)
    {
        // lexing
        <#LEXER#>_MainLexer scanner = new <#LEXER#>_MainLexer();
        var lexerResult = scanner.Scan(source.AsSpan());
            
        if (lexerResult.IsError)
        {        
            return new ParseResult<<#LEXER#>, <#OUTPUT#>>(lexerResult.Error);
        }


        // parsing
        var mainTokens = lexerResult.MainTokens;
        var parser = new Static<#PARSER#>();
        var result = parser.ParseNonTerminal_<#ROOT#>(mainTokens, 0, new ParsingContext<<#LEXER#>, <#OUTPUT#>>(_useMemoization));
        if (result.IsOk)
        {
            if (result.EndingPosition < mainTokens.Count - 1)
            {
                ParseResult<<#LEXER#>, <#OUTPUT#>> notEndedResult = new ParseResult<<#LEXER#>, <#OUTPUT#>>(result.GetCompactedParseErrors());
                return notEndedResult;
            }
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

