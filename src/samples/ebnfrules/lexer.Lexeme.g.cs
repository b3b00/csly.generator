using System;
using System.Collections.Generic;
using System.Text;

namespace csly.models
{

    internal class Lexeme
    {
        private readonly GenericToken _type;
        private readonly string _name;
        private readonly string[] _args;

        public GenericToken Type => _type;
        public string Name => _name;
        public string[] Args => _args;

        public Lexeme(GenericToken type, string name, params string[] args)
        {
            _type = type;
            _name = name;
            _args = args;
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
