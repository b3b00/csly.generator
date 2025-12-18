using System;

namespace <#NS#> {


    public class FsmMatch<T>
    {
        public bool IsMatch { get; set; } = false;

        public bool IsDone { get; set; } = false;

        public T Token { get; set; }

        public ReadOnlyMemory<char> Value { get; set; }

        public LexerPosition Position { get; set; }
        public CommentType CommentType  => (IsMultiLineComment) ? CommentType.Multi : (IsSingleLineComment) ? CommentType.Single : CommentType.No;

        public bool IsExplicit { get; set; } = false;

        public bool IsSingleLineComment { get; set; } = false;

        public bool IsMultiLineComment { get; set; } = false;

        public string MultiLineCommentEndDelimiter { get; set; } = null;

        public bool IsPop { get; set; } = false;

        public string PushTarget { get; set; } = null;

        public FsmMatch(ReadOnlyMemory<char> value, LexerPosition position)
        {
            Value = value;
            Position = position;
            IsExplicit = true;
            IsMatch = true;
        }

        public FsmMatch(T token, ReadOnlyMemory<char> value, LexerPosition position)
        {
            Token = token;
            Value = value;
            Position = position;
            IsMatch = true;
        }

        public FsmMatch()
        {
            IsMatch = false;
        }
    }
}