using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;


namespace ebnf.grammar
{

    public class Rule : GrammarNode
    {
        public Rule()
        {
            Clauses = new List<IClause>();
            _tokenToVisitorMethodName = new Dictionary<string, string>();
            MethodName = null;
            IsSubRule = false;
            NodeName = "";
        }

        public Rule(string head, List<IClause> clauses, string methodName) : this()
        {
            NonTerminalName = head;
            Clauses = clauses;
            MethodName = methodName;
        }

        public string NodeName { get; set; } = null;

        public string[] SubNodeNames { get; set; } = null;

        private Dictionary<string,string> _tokenToVisitorMethodName = new Dictionary<string, string>();

        public Dictionary<string, string> TokenToVisitorMethodName => _tokenToVisitorMethodName;

        private Dictionary<string, string> NodeNamesMap { get; set; } = new Dictionary<string, string>();

        public bool IsByPassRule { get; set; } = false;

       
        // visitor for classical rules
        public string MethodName { get; set; }

        public bool IsExpressionRule { get; set; }

        public bool IsInfixExpressionRule { get; set; }

        public Affix ExpressionAffix { get; set; }

        public string RuleString { get; set; }

        public List<IClause> Clauses { get; set; }
        public List<string> Leaders { get; set; } = new List<string>();

        public string NonTerminalName { get; set; }

        public string Head => NonTerminalName;

        public string Name => NonTerminalName;

        public bool ContainsSubRule
        {
            get
            {
                if (Clauses != null && Clauses.Any())
                {
                    bool contains = false;
                    foreach (var clause in Clauses)
                    {
                        switch (clause)
                        {
                            case GroupClause _:
                                contains = true;
                                break;
                            case ManyClause many:
                                contains |= many.manyClause is GroupClause;
                                break;
                            case OptionClause option:
                                contains |= option.Clause is GroupClause;
                                break;
                        }

                        if (contains)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public bool IsSubRule { get; set; }

        public bool MayBeEmpty => Clauses == null
                                  || Clauses.Count == 0
                                  || Clauses.Count == 1 && Clauses[0].MayBeEmpty();

        public bool ForcedName { get; set; }
        public Associativity Associativity { get; internal set; }
        public int Precedence { get; internal set; }

        public string GetVisitorMethodName(string token)
        {
            
            if (IsExpressionRule)
            {
                if (_tokenToVisitorMethodName.ContainsKey(token))
                {
                    return _tokenToVisitorMethodName[token];
                }                
            }

            return MethodName;
        }

        public void SetVisitorMethodName(string token, string methodName)
        {
            if (IsExpressionRule)
            {
                _tokenToVisitorMethodName[token] = methodName;                
            }
        }


        public void SetVisitor(string methodName)
        {
            MethodName = methodName;
        }


        public string Dump()
        {
            return $"Rule: {NonTerminalName} : {string.Join(" ",Clauses.Select(x => x.Dump()))}";
        }
    }
}