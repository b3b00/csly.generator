using System;
using System.Diagnostics.CodeAnalysis;

namespace ebnf.grammar
{

    public sealed class OptionClause : IClause
    {
        public OptionClause(IClause clause)
        {
            Clause = clause;
        }

        public IClause Clause { get; set; }

        public bool IsGroupOption => Clause is NonTerminalClause clause && clause.IsGroup;

        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool MayBeEmpty()
        {
            return true;
        }


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{Clause}?";
        }


        public bool Equals(IClause clause)
        {
            if (clause is OptionClause other)
            {
                return Equals(other);
            }
            return false;
        }

        private bool Equals(OptionClause other)
        {
            return Equals(Clause, other.Clause);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OptionClause)obj);
        }

        public override int GetHashCode()
        {
            return (Clause != null ? Clause.GetHashCode() : 0);
        }

        public string Dump() {
            return $"{Clause}?";
        }
    }
}