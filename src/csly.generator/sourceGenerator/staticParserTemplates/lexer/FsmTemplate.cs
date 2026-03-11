
using System.Collections.Generic;
using Factory = System.Func<csly.<#ASSEMBLY#>.models.FsmMatch<<#NAMESPACE#>.<#LEXER#>>, csly.<#ASSEMBLY#>.models.Token<<#NAMESPACE#>.<#LEXER#>>>;

namespace <#NAMESPACE#>;


    

public class <#LEXER#>_FsmLexer_<#MODE#> : ISubLexer
{

    public string Name => "<#MODE#>";

    private int _currentState = 0;

    private FsmMatch<<#LEXER#>> _currentMatch = null;

    private FsmMatch<<#LEXER#>> _lastSuccessMatch = null;

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


    private List<int> _epsilonStates = new List<int>()
    {
        <#EPSILON_STATES#>
    };

    private Dictionary<string,<#LEXER#>> _keywords = new Dictionary<string,<#LEXER#>>()        
    {
        <#KEYWORDS#>
    };

        private Dictionary<int, <#LEXER#>> _stateTokens = new Dictionary<int, <#LEXER#>>()
    {
        <#STATE_TOKENS#>
    };

    private List<string> _explicitKeywords = new List<string>()
    {
        <#EXPLICIT_KEYWORDS#>
    };

    private Dictionary<string, <#LEXER#>> _uptoTOkens = new Dictionary<string, <#LEXER#>>()
    {
        <#UPTOS#>
    };

    private Dictionary<<#LEXER#>, Factory> _tokenFactories = new Dictionary<<#LEXER#>, Factory>();

private Factory _defaultFactory;

    public <#LEXER#>_FsmLexer_<#MODE#>()
    {
        _defaultFactory = match => new Token<<#LEXER#>>(match.Token, match.Value, match.Position, match.CommentType, match.MultiLineCommentEndDelimiter) {
            IsExplicit = match.IsExplicit,
            IsPop = match.IsPop,
            PushTarget = match.PushTarget
        };
        <#FACTORIES#>
    }

    

    /// consumes all whitspaces starting from _currentPosition and move _currentPosition accordingly
    
    public List<char> GetIndentations(ReadOnlySpan<char> source, int index)
    {
        var c = source[index];
        List<Char> indentations = new List<char>();
        int i = 0;
        if (index >= source.Length)
        {
            return new List<char>();
        }

        char current = source[index + i];
        while (i < source.Length && (current == ' ' || current == '\t'))
        {
            indentations.Add(current);
            i++;
            current = source[index + i];
        }

        return indentations;
    }

    // INDENT indentations
    private Token<<#LEXER#>> ConsumeIndents3(ReadOnlySpan<char> source, LexerPosition lexerPosition)
        {   
            if (lexerPosition.IsStartOfLine && lexerPosition.Index < source.Length)
            {
                var shifts = GetIndentations(source, lexerPosition.Index);
                string currentShift = string.Join("", shifts);
                lexerPosition.Indentation = lexerPosition.Indentation ?? new LexerIndentation();
                var indentation = lexerPosition.Indentation.Indent(currentShift);
                switch (indentation.type)
                {
                    case LexerIndentationType.Indent:
                    {
                        var position = lexerPosition.Clone();
                        position.IsPush = false;
                        position.IsPop = false;
                        position.Mode = null;
                        position.Index += currentShift.Length;
                        position.Column += currentShift.Length;
                        
                        var indent = new Token<<#LEXER#>>
                        {
                            IsIndent = true,
                            IsUnIndent = false,
                            IsNoIndent = false,
                            Position = position,
                            IsEOS = false
                        };
                        return indent;
                    }
                    case LexerIndentationType.UIndent:
                    {
                        var position = lexerPosition.Clone();
                        position.IsPush = false;
                        position.IsPop = false;
                        position.Mode = null;
                        position.Index += currentShift.Length;
                        position.Column += currentShift.Length;
                        
                        var indent = new Token<<#LEXER#>>
                        {
                            IsIndent = false,
                            IsUnIndent = true,
                            IsNoIndent = false,
                            Position = position,
                            IsEOS = false
                        };
                        return indent;
                    }
                    case LexerIndentationType.None:
                    {
                        var position = lexerPosition.Clone();
                        position.IsPush = false;
                        position.IsPop = false;
                        position.Mode = null;
                        position.Index += currentShift.Length;
                        position.Column += currentShift.Length;
                        
                        var indent = new Token<<#LEXER#>>
                        {
                            IsIndent = false,
                            IsUnIndent = false,
                            IsNoIndent = true,
                            Position = position
                        };
                        return indent;
                    }
                    case LexerIndentationType.Error:
                    {
                        // INDENT handle indentation error
                        var position = lexerPosition.Clone();
                        position.IsPush = false;
                        position.IsPop = false;
                        position.Mode = null;
                        position.Index += currentShift.Length;
                        position.Column += currentShift.Length;
                        
                        var indent = new Token<<#LEXER#>>
                        {
                            IsIndent = false,
                            IsUnIndent = false,
                            IsNoIndent = false,
                            Position = position
                        };
                        return indent;
                    }
                } 
            }

            return new Token<<#LEXER#>>()
            {
                IsIndent = false,
                IsUnIndent = false,
                IsNoIndent = true,
                Position = lexerPosition.Clone()
            };
        }
    private void ConsumeWhiteSpace(ReadOnlySpan<char> source)
    {
        while (_currentPosition.Index < source.Length && char.IsWhiteSpace(source[_currentPosition.Index]))
        {
            if (source[_currentPosition.Index] == '\n')
            {
                _currentPosition.Line++;
                _currentPosition.Column = 0;
                _currentPosition.Index++;
                
                bool isNextWhite = _currentPosition.Index+1 < source.Length ? 
                    (source[_currentPosition.Index + 1] == ' ' || source[_currentPosition.Index + 1] == '\t')
                    : true; 
                
                if (<#IS_INDENTATION_AWARE#> && isNextWhite) // TODO indentation aware : stop consuming white spaces if we are at the start of a new line, indents will be consumed separately
                {
                    break;
                }
            }
            else
            {
                _currentPosition.Column++;
            }
            _currentPosition.Index++;
            _startPosition.Index = _currentPosition.Index;
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




public (LexerResult<<#LEXER#>> Result, LexerPosition NewPosition, bool isPop, string PushTarget) Scan(ReadOnlySpan<char> source, LexerPosition position) {
        _currentPosition = position;
_startPosition = position.Clone();
List <Token<<#LEXER#>>> tokens = new List<Token<<#LEXER#>>>();


        void AddToken(Token <<#LEXER#>> token) {
            Console.WriteLine("\t"+token);
            _lastSuccessMatch = null;
            _currentMatch = null;
            tokens.Add(token);
        }

        bool IsModeChanging()
        {
            if (tokens.Count == 0)
                return false;
            var lastToken = tokens[tokens.Count - 1];
            return lastToken.IsPop || !string.IsNullOrEmpty(lastToken.PushTarget);
        }

        <#CONSUME_INDENTS#>
        
        ConsumeWhiteSpace(source);
        
        <#CONSUME_INDENTS#>
        
       

        while ((_currentPosition.Index < source.Length || _epsilonStates.Contains(_currentState)) && !IsModeChanging())
        {   
            <#STATE_CALLS#>
        }
        if (_lastSuccessMatch != null)
        {
            //final token to add
            Func<FsmMatch <<#LEXER#>>, Token<<#LEXER#>>> factory;

            if (!_tokenFactories.TryGetValue(_lastSuccessMatch.Token, out factory))
            {
                factory = _defaultFactory;
            }
            var token = factory(_lastSuccessMatch);

            _currentPosition = ConsumeComments(token, source.ToArray());
            
            AddToken(token);
            _lastSuccessMatch = null;
        }
var lastToken = tokens.Count > 0 ? tokens[tokens.Count - 1] : null;
        bool isPop = lastToken != null && (lastToken.IsPop);
        string pushTarget = lastToken != null ? lastToken.PushTarget : null;
        return (tokens, _currentPosition, isPop, pushTarget);
    }


    <#STATES#>
}