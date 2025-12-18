
using System.Collections.Generic;
using Factory = System.Func<csly.<#ASSEMBLY#>.models.FsmMatch<<#NAMESPACE#>.<#LEXER#>>, csly.<#ASSEMBLY#>.models.Token<<#NAMESPACE#>.<#LEXER#>>>;

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

        private List<string> _explicitKeywords = new List<string>()
        {
            <#EXPLICIT_KEYWORDS#>
        };

    private Dictionary<<#LEXER#>, Factory> _tokenFactories = new Dictionary<<#LEXER#>, Factory>();

private Factory _defaultFactory;

    public <#LEXER#>_FsmLexer()
    {
        _defaultFactory = match => new Token<<#LEXER#>>(match.Token, match.Value, match.Position, match.CommentType, match.MultiLineCommentEndDelimiter) {
            IsExplicit = match.IsExplicit,
            IsPop = match.IsPop,
            PushTarget = match.PushTarget
        };
        <#FACTORIES#>
    }

    

    /// consumes all whitspaces starting from _currentPosition and move _currentPosition accordingly
    private void ConsumeWhiteSpace(ReadOnlySpan<char> source)
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

    private LexerPosition ConsumeComments(Token<<#LEXER#>> comment, ReadOnlyMemory<char> source)
    {
        // TODO return the value and new position after consuming comment
        var lexerPosition = comment.Position;
        ReadOnlyMemory<char> commentValue;

        if (comment.CommentType == CommentType.Single)
        {
            // single line comment
            var position = lexerPosition.Index;
                if (position < source.Length -1)
                {
                    commentValue = source.GetToEndOfLine(position);
                }
                else
                {
                    commentValue = "".ToCharArray();
                }
                comment.Channel = 1;
                position = position + commentValue.Length;
                if (commentValue.Length > comment.SpanValue.Length)
                {
                    comment.SpanValue = commentValue.RemoveEndOfLineChars();
                }
                else
                {
                    comment.SpanValue = "".ToCharArray();
                }

                var newPosition = lexerPosition.Clone();
                newPosition.Index = position;
                newPosition.Line++;
                newPosition.Column = 0;
                return newPosition;
        }
        else if (comment.CommentType == CommentType.Multi)
        {
            // multi line comment
            var position = lexerPosition.Index;
                var end = source.Span.Slice(position).IndexOf<char>(comment.MultiLineCommentEndDelimiter.AsSpan());
                if (end < 0)
                    position = source.Length;
                else
                    position = end + position;
                commentValue = source.Slice(lexerPosition.Index, position - lexerPosition.Index);
                comment.SpanValue = commentValue;
                comment.Channel = 1;
                var newPosition = lexerPosition.Index + commentValue.Length + comment.MultiLineCommentEndDelimiter.Length;
                var lines = EOLManager.GetLinesLength(commentValue);
                var newLine = lexerPosition.Line + lines.Count - 1;
                int newColumn;
                if (lines.Count > 1)
                    newColumn = lines[lines.Count-1] + comment.MultiLineCommentEndDelimiter.Length;
                else
                    newColumn = lexerPosition.Column + lines[0] + comment.MultiLineCommentEndDelimiter.Length;

                var newLexerPosition = lexerPosition.Clone();
                newLexerPosition.Index = newPosition;
                newLexerPosition.Line = newLine;
                newLexerPosition.Column = newColumn;
                return newLexerPosition;  
        }
        return _currentPosition;
    }


    public LexerResult<<#LEXER#>> Scan(ReadOnlySpan<char> source) {
        _currentPosition = new LexerPosition(0,0,0);
_startPosition = new LexerPosition(0,0,0);
List<Token<<#LEXER#>>> tokens = new List<Token<<#LEXER#>>>();


        void AddToken(Token<<#LEXER#>> token) {
            tokens.Add(token);
        }
        ConsumeWhiteSpace(source);
        while (_currentPosition.Index <= source.Length)
        {   
            <#STATE_CALLS#>
        }
        return tokens;
    }


    <#STATES#>
}