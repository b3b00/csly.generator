
namespace ebnf.grammar
{

    public class NonTerminalClause : AbstractClause
    {

        public bool IsGroup { get; set; } = false;

        public NonTerminalClause(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        private bool _mayBeEmpty = false;

        public override bool MayBeEmpty() => _mayBeEmpty;

        public bool SetMayBeEmpty(bool mayBeEmpty)
        {
            bool setted = mayBeEmpty && !_mayBeEmpty;
            _mayBeEmpty = mayBeEmpty;
            return setted;
        }
    }
}