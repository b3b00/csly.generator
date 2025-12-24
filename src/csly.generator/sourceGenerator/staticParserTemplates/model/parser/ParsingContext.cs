using System;
using System.Collections.Generic;
using System.Text;

namespace <#NS#>
{
    public class ParsingContext<IN, OUT> where IN : struct, Enum
    {

        private readonly Dictionary<string, SyntaxParseResult<IN, OUT>> _memoizedNonTerminalResults = new Dictionary<string, SyntaxParseResult<IN, OUT>>();

        private readonly bool _useMemoization = false;
        public ParsingContext(bool useMemoization)
        {
            _useMemoization = useMemoization;
        }

        private string GetKey(IClause clause, int position)
        {
            return $"{clause.Name} -- @{position}";
        }

        public void Memoize(IClause clause, int position, SyntaxParseResult<IN, OUT> result)
        {
            if (_useMemoization)
            {
                _memoizedNonTerminalResults[GetKey(clause, position)] = result;
            }
        }

        public bool TryGetParseResult(IClause clause, int position, out SyntaxParseResult<IN, OUT> result)
        {
            if (!_useMemoization)
            {
                result = null;
                return false;
            }
            bool found = _memoizedNonTerminalResults.TryGetValue(GetKey(clause, position), out result);
            return found;
        }

    }
}
