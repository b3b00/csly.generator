using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ebnf.grammar
{

    public sealed class ClauseSequence : GrammarNode
    {
        public ClauseSequence(IClause item)
        {
            Clauses = new List<IClause>();
            Clauses.Add(item);
        }

        public List<IClause> Clauses { get; set; }

        public bool MayBeEmpty()
        {
            return true;
        }


        public void AddRange(List<IClause> clauses)
        {
            Clauses.AddRange(clauses);
        }

        public void AddRange(ClauseSequence seq)
        {
            AddRange(seq.Clauses);
        }
    }
}