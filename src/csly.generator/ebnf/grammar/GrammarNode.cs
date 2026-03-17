using System;

namespace ebnf.grammar
{

    public interface GrammarNode
    {
        public GrammarNode Parent { get; set; }
        public bool IsRoot { get; }

        public GrammarNode Root { get; }
        
        string Dump();
    }
}