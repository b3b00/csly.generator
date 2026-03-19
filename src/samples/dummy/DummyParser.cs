using csly.dummy.dummyparser.models;

namespace Tester;



[ParserGenerator]

public partial class TesterMain : AbstractParserGenerator<ABCToken, DummyParser, string>
{

}


[ParserRoot("root")]
public class DummyParser
{
    [Production("root : nullable B? C")]
    public string Root(string nullable, Token<ABCToken> b, Token<ABCToken> c)
    {
        return $"a={nullable} b={(b.IsEmpty?"<none>":b.Value)} c={c.Value}";
    }

    [Production("nullable: A?")]
    public string Nullable(Token<ABCToken> a)
    {
        return a.IsEmpty ? "<none>" : a.Value.ToString();
    }

}
    
public enum ABCToken
{
    [AlphaId]
    ID, 
    
    [Keyword("A")]
    A,
    [Keyword("B")]
    B,
    [Keyword("C")]
    C,  
}

