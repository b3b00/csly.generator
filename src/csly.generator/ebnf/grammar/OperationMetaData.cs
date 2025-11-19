using System;


namespace ebnf.grammar
{


    public class OperationMetaData
    {
        public OperationMetaData(int precedence, Associativity assoc, string methodName, Affix affix, string oper, string nodeName = null)
        {
            // TODO : check if oper is explicit
            Precedence = precedence;
            Associativity = assoc;
            MethodName = methodName;
            OperatorToken = oper;
            Affix = affix;
            NodeName = nodeName;
        }






        public int Precedence { get; set; }

        public Associativity Associativity { get; set; }

        public string MethodName { get; set; }

        public string OperatorToken { get; set; }

        public string Operatorkey => (IsExplicitOperatorToken ? ExplicitOperatorToken : OperatorToken.ToString());

        public Affix Affix { get; set; }

        public string NodeName { get; set; }

        public bool IsBinary => Affix == Affix.InFix;

        public bool IsUnary => Affix != Affix.InFix;

        public bool IsExplicitOperatorToken => !string.IsNullOrEmpty(ExplicitOperatorToken);

        public string ExplicitOperatorToken { get; set; }


    }
}