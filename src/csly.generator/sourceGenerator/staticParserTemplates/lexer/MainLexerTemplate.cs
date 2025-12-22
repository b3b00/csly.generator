
using System.Collections.Generic;
using csly.<#ASSEMBLY#>.models;


namespace <#NAMESPACE#>;


    

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
            //Console.WriteLine($"\n==============================");
            //Console.WriteLine($"=== #{count++} ====");
            //Console.WriteLine($"=== ");
            //Console.WriteLine($"\n=== mode [{lexerStack.Peek().Name}]");
            //Console.WriteLine($"=== position {position} ");
            //Console.WriteLine($"=== Index={position.Index} / {source.Length}");
            //Console.WriteLine($"=== remaining source: '{source.Slice(position.Index).ToString()}'");
            //Console.WriteLine($"==============================");
            //Console.WriteLine($"=== {string.Join(", ", tokens.Select(x => "[" + x.Value.ToString() + "]"))}");
            //Console.WriteLine($"==============================");
            var currentLexer = lexerStack.Peek();
    var scanResult = currentLexer.Scan(source, position);
    var result = scanResult.Result;

            if (result.IsError)
            {
                return result;
            }

// add tokens
//Console.WriteLine($"Lexer mode {currentLexer.Name} produced {result.Tokens.Count} tokens ");
//    Console.WriteLine($"Tokens: \n- {string.Join("\n- ", result.Tokens.Select(x => x.ToString()))}");
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
                //Console.WriteLine($"<<< popping to mode [{lexerStack.Peek().Name}] ");

            }
            else if (!string.IsNullOrEmpty(scanResult.PushTarget))
{
    //Console.WriteLine($">>> Pushing lexer mode to [{scanResult.PushTarget}] ");
    if (!SubLexers.ContainsKey(scanResult.PushTarget))
    {
        return new LexicalError($"Unknown lexer mode to push: {scanResult.PushTarget}");
    }
    lexerStack.Push(SubLexers[scanResult.PushTarget]);
}
        }

        tokens.Add(new Token<<#LEXER#>>());
        return tokens;
    } 

}