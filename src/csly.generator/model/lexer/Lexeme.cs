using csly.ebnf.models;
using csly.generator.sourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csly.generator.model.lexer
{
    internal class Lexeme
    {
        private readonly GenericToken _type;
        private readonly string _name;
        private readonly string[] _args;

        public GenericToken Type => _type;
        public string Name => _name;
        public string[] Args => _args;

        public string Arg0 => Args != null && Args.Any() ? Args.First().TrimQuotes() : null;

        public List<string> Modes { get; internal set; }

        public string PushTarget {get; set;}

        public bool IsPop {get; set;}

        public bool IsPush => !string.IsNullOrEmpty(PushTarget);

        public bool IsExplicit { get; set; } = false;

        public IEnumerable<char[]> IdentifierStartPatterns()
        {
            if (Args == null || Args.Length > 0)
            {
                return ParseIdentifierPattern(Args[1]).ToList();
            }
            return Enumerable.Empty<char[]>();
        }

        public IEnumerable<char[]> IdentifierTailPatterns()
        {
            if (Args == null || Args.Length > 1)
            {
                return ParseIdentifierPattern(Args[1]).ToList();
            }
            return Enumerable.Empty<char[]>();
        }

        private static IEnumerable<char[]> ParseIdentifierPattern(string pattern)
        {
            var index = 0;
            while (index < pattern.Length)
            {
                if (index <= pattern.Length - 3 && pattern[index + 1] == '-')
                {
                    if (pattern[index] < pattern[index + 2])
                    {
                        yield return new[] { pattern[index], pattern[index + 2] };
                    }
                    else
                    {
                        yield return new[] { pattern[index + 2], pattern[index] };
                    }

                    index += 3;
                }
                else
                {
                    yield return new[] { pattern[index++] };
                }
            }
        }

        public Lexeme(GenericToken type, string name, params string[] args)
        {
            _type = type;
            _name = name;
            _args = args;
        }

        public Lexeme(GenericToken type, string pattern)
        {
            _type = type;
            _name = null;
            _args = new[] { pattern };
            Modes = new();
            IsExplicit = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Lexeme: Type={_type}, Name={_name}");
            if (_args != null && _args.Length > 0)
            {
                sb.Append($", Args=[{string.Join(", ", _args)}]");
            }
            return sb.ToString();
        }
    }
}
