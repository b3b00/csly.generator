using csly.models;

namespace extending;

public enum Toky
{
    [AlphaId] ID,
    [Keyword("A")] A,
    [Keyword("B")] B,
    [Keyword("C")] C
}


[ParserRoot("root")]
public class ExtParser {

    [Production("root : a bs c")]
    public string Root(string a , string bs, string c)     {
        return a + " " + bs + " " + c;
    }

    [Production("a : A")]
    public string A(Token<Toky> a)
    {
               return a.Value;
    }

    [Production("bs : B*")]
    public string Bs(List<Token<Toky>> bs)
    {
        return string.Join(" ", bs.Select(b => b.Value));
    }

    [Production("c : C")]
    public string C(Token<Toky> c)
    {
        return c.Value;
    }

}

//[ParserGenerator]
public partial class Tokynou : AbstractParserGenerator<Toky, ExtParser, string>
{
}