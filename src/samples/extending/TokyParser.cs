using csly.models;

namespace extending
{
    [ParserRoot("root")]
    public class ExtParser
    {

        [Production("root : a b g")]
        public string Root(string a, string b, string g)
        {
            return a + " - " + b + " - " + g;
        }

        [Production("a : [A | AA ]")]
        public string A(Token<Toky> a)
        {
            return a.TokenID == Toky.A ? "ah ah" : "aah aah";
        }

        [Production("b : [b1 | b2]")]
        public string B(string b) => b;

        [Production("b1 : B")]
        public string B1(Token<Toky> b) => "🐝";

        [Production("b2 : D")]
        public string B2(Token<Toky> d) => "🦆";

        [Production("g : GT[d] ( a X )+ LT[d] GT[d] ( X[d] a )? LT[d] ")]
        //public string G(Token<Toky> gt, List<Group<Toky, string>> gs, Token<Toky> lt, Token<Toky> gt2, ValueOption<Group<Toky, string>> optionGroup, Token<Toky> lt2)
        public string G(List<Group<Toky, string>> gs,  ValueOption<Group<Toky, string>> optionGroup)
        {

            var groups = string.Join(" , ", gs.Select(g => "(" + g.Value(0) + " , " + g.Token(1).Value + ")"));
            string opt = "";
            var og = optionGroup.Match((x) =>   x.Value(0), () => "🫗");            
            return $"brackets( {groups} ) - ?{{ {og} }}";
        }

        [Operation("PLUS",Affix.InFix,Associativity.Left,10)]
        public string Plus(string left, Token<Toky> op, string right)
        {
            return $"( {left} + {right} )";
        }

        [Operation("MINUS", Affix.InFix, Associativity.Left, 10)]
        public string Minus(string left, Token<Toky> op, string right)
        {
            return $"( {left} + {right} )";
        }

        [Operand]
        [Production("int : INT")]
        public string intOperand(Token<Toky> intToken)
        {
            return intToken.Value;
        }


    }
}
