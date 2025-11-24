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
            return a.Value;
        }

        [Production("bs : B* D")]
        public string Bs(List<Token<Toky>> bs, Token<Toky> d)
        {
            string bees = bs.Count > 0 ? "Bs { " + string.Join(", ", bs.Select(b => b.Value)) + " }" : "no Bees";
            return $"{bees}, {d.Value}";
        }

        [Production("c : C")]
        public string C(Token<Toky> c)
        {
            return c.Value;
        }

    }
}
