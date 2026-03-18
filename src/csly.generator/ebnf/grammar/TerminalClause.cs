using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ebnf.grammar
{

    public class TerminalClause : AbstractClause
    {

        private bool _isExplicit;
        
        protected bool _isIndent;
        
        protected bool _isUIndent;
        public bool Discarded { get; set; }

        public bool IsExplicit => _isExplicit;
        
        public bool IsIndent => _isIndent;
        
        public bool IsUIndent => _isUIndent;
        

        public string ExplicitValue { get; set; }
        public TerminalClause(string name, bool discarded = false)
        {
            Name = name;
            if (Name.StartsWith("'") && Name.EndsWith("'"))
            {
                _isExplicit = true;
                Name = Name.Substring(1, Name.Length - 2);
            }
            if (Name == "INDENT")
            {
                _isIndent = true;
            }
            else if (Name == "UINDENT")
            {
                _isUIndent = true;
            }
            Discarded = discarded;
        }

        public TerminalClause(string name, string explicitValue, bool discarded = false)
        {
            Name = name;
            ExplicitValue = explicitValue.Trim('\'');
            _isExplicit = true;
            Discarded = discarded;
        }

        public override string ToString()
        {
            return _isExplicit ? $"'{Name}'" : Name;
        }

        public override bool MayBeEmpty() => false;

        public override string Dump()
        {
            return Name;
        }
    }

    public sealed class IndentTerminalClause : TerminalClause
    {
        private IndentationType ExpectedIndentation;

        public IndentTerminalClause(IndentationType expectedIndentation, bool discard) : base(IndentationType.Indent.ToString())
        {
            ExpectedIndentation = expectedIndentation;
            _isIndent = expectedIndentation == IndentationType.Indent;
            _isUIndent = expectedIndentation == IndentationType.UnIndent;
            Discarded = discard;
        }

        public override bool MayBeEmpty()
        {
            return false;
        }


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append(ExpectedIndentation == IndentationType.Indent ? "INDENT" : "UINDENT");
            if (Discarded) b.Append("[d]");
            return b.ToString();
        }

        public string Dump() => ToString();

    }
}