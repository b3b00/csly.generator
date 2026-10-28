using csly.Tester.testerparser.models;

namespace csly.generator.tester;

[ParserRoot("root")]
public class TesterParser
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