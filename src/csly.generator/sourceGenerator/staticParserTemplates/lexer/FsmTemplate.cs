
using System.Collections.Generic;
using Factory = System.Func<csly.models.FsmMatch<<#NAMESPACE#>.<#LEXER#>>, csly.models.Token<<#NAMESPACE#>.<#LEXER#>>>;

namespace <#NAMESPACE#>;

    

public class <#LEXER#>_FsmLexer
{

    private int _currentState = 0;

    private FsmMatch<<#LEXER#>> _currentMatch = null;

    private LexerPosition _currentPosition { get; set; }

    private LexerPosition _startPosition { get; set; }

    private char GetChar(ReadOnlySpan<char> source, LexerPosition position)
    {
        if (position.Index >= source.Length)
        {
            return '\0';
        }
        return source[position.Index];
    }

    private Dictionary<string,<#LEXER#>> _keywords = new Dictionary<string,<#LEXER#>>()        
    {
        <#KEYWORDS#>
    };

    private Dictionary<<#LEXER#>, Factory> _tokenFactories = new Dictionary<<#LEXER#>, Factory>();

private Factory _defaultFactory;

    public <#LEXER#>_FsmLexer()
    {
        _defaultFactory = match => new Token<<#LEXER#>>(match.Token, match.Value, match.Position);     
        <#FACTORIES#>
    }

    

    /// consumes all whitspaces starting from _currentPosition and move _currentPosition accordingly
    private void ConsumeWhitSpace(ReadOnlySpan<char> source)
    {
        while (_currentPosition.Index < source.Length && char.IsWhiteSpace(source[_currentPosition.Index]))
        {
            if (source[_currentPosition.Index] == '\n')
            {
                _currentPosition.Line++;
                _currentPosition.Column = 0;
            }
            else
            {
                _currentPosition.Column++;
            }
            _currentPosition.Index++;
        }
    }

    public LexerResult<<#LEXER#>> Scan(ReadOnlySpan<char> source) {
        _currentPosition = new LexerPosition(0,0,0);
        _startPosition = new LexerPosition(0,0,0);
        List<Token<<#LEXER#>>> tokens = new List<Token<<#LEXER#>>>();
        while (_currentPosition.Index <= source.Length)
        {
            
            

            <#STATE_CALLS#>
        }
        return tokens;
    }


    <#STATES#>
}