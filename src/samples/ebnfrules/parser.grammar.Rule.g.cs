using System.Collections.Generic;

namespace csly.ebnf.models
{

    public class Rule
    {
        public string Head { get; set; }

        public List<IClause> Clauses { get; set; }

        public List<string> Leaders { get; set; } = new List<string>();

        public string Name => _name;

        private readonly string _name;

        public Rule(string head, List<IClause> clauses)
        {
            this.Head = head;
            this.Clauses = clauses;
            _name = head;
        }

        public override string ToString()
        {
            return $"{Head} -> {string.Join(" ", Clauses)}";
        }
    }
}