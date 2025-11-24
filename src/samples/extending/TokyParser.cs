using csly.models;

namespace extending
{
    [ParserRoot("root")]
    public class ExtParser
    {

        [Production("root : a bs c")]
        public string Root(string a, string bs, string c)
        {
            return a + " " + bs + " " + c;
        }

        [Production("a : A")]
        public string A(Token<Toky> a)
        {
            return "ah ah";
        }

        [Production("bs : B* D")]
        public string Bs(List<Token<Toky>> bs, Token<Toky> d)
        {
            string bees = bs.Count > 0 ? $"{bs.Count} 🐝" : "no 🐝";
            return $"{bees}, lady di";
        }

        [Production("c : C")]
        public string C(Token<Toky> c)
        {
            return "si!";
        }

    }
}
