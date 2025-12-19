
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
        List<Token<WhileTokenGeneric>> tokens = new List<Token<WhileTokenGeneric>>();

        Stack<ISubLexer> lexerStack = new Stack<ISubLexer>();

        // start with default lexer
        lexerStack.Push(SubLexers["default"]);

        while (true)
        {
            break;
        }

        // TODO : main loop
        return tokens;
    } 

}