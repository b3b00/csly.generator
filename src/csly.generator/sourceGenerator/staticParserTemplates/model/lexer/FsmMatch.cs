using System;

namespace <#NS#> {


    public class FsmMatch<T>  where T : struct, Enum
    {
        public bool IsMatch { get; set; } = false;

        public bool IsDone { get; set; } = false;

        public T Token { get; set; }
        
        public string TokenName { get; set; }
        
        public bool IsShadowId { get; private set; }

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
            TokenName = token.ToString();
        }
        
        public FsmMatch(string tokenName, ReadOnlyMemory<char> value, LexerPosition position)
        {
            Token = default(T);
            Value = value;
            Position = position;
            IsMatch = true;
            TokenName = tokenName;
            IsShadowId = TokenName == Constants.ShadowId;
            if (Enum.TryParse<T>(TokenName, out var tok))
            {
                
                Token = tok;
            }
        }

        public FsmMatch()
        {
            IsMatch = false;
        }

        public FsmMatch<T> Clone()
        {
            return new FsmMatch<T>()
            {
                IsMatch = this.IsMatch,
                IsDone = this.IsDone,
                Token = this.Token,
                TokenName = this.TokenName,
                IsShadowId = this.IsShadowId,
                Value = this.Value,
                Position = this.Position,
                IsExplicit = this.IsExplicit,
                IsSingleLineComment = this.IsSingleLineComment,
                IsMultiLineComment = this.IsMultiLineComment,
                MultiLineCommentEndDelimiter = this.MultiLineCommentEndDelimiter,
                IsPop = this.IsPop,
                PushTarget = this.PushTarget
            };
        }
}
}