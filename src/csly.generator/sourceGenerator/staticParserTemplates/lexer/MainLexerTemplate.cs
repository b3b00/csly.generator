
using System.Collections.Generic;
using csly.<#ASSEMBLY#>.<#PARSER_NS#>.models;
using System;


namespace <#NAMESPACE#>.<#PARSER_NS#>;


    

public class <#LEXER#>_MainLexer
{

    Dictionary<string, ISubLexer> SubLexers = new Dictionary<string, ISubLexer>()
        {
            <#SUB_LEXERS#>
        };

    public LexerResult<<#LEXER#>> Scan(ReadOnlySpan<char> source)
    {
        List<Token<<#LEXER#>>> tokens = new List<Token<<#LEXER#>>>();

    Stack<ISubLexer> lexerStack = new Stack<ISubLexer>();

    // start with default lexer
    lexerStack.Push(SubLexers["default"]);

        LexerPosition position = new LexerPosition(0, 0, 0);
        int count = 0;
        while (position.Index<source.Length)
        {
            var currentLexer = lexerStack.Peek();
    var scanResult = currentLexer.Scan(source, position);
    var result = scanResult.Result;

            if (result.IsError)
            {
                return result;
            }

tokens.AddRange(result.Tokens);

            // update position
            position = scanResult.NewPosition;

            // handle push/pop
            if (scanResult.isPop)
            {
                if (lexerStack.Count == 0)
                {
                    // cannot pop the last lexer
                    return new LexicalError("Cannot pop the last lexer");
                }
                lexerStack.Pop();

            }
            else if (!string.IsNullOrEmpty(scanResult.PushTarget))
{
    if (!SubLexers.ContainsKey(scanResult.PushTarget))
    {
        return new LexicalError($"Unknown lexer mode to push: {scanResult.PushTarget}");
    }
    lexerStack.Push(SubLexers[scanResult.PushTarget]);
}
        }

        <#AUTO_CLOSE#>
        
        tokens.Add(Token<<#LEXER#>>.EOS());
        return tokens;
    } 

}