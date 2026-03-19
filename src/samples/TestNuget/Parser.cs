using System.Text;
using csly.testnuget.theparser.models;

namespace TestNuget;

[ParserGenerator]
public partial class TheParserGenerator : AbstractParserGenerator<TheLexer, TheParser, string>{
    
}

[ParserRoot("root")] // do not forget !
public class TheParser
{
    [Production("root : ID COLON[d] ints")]
    public string Root(Token<TheLexer> id, string ints)
    {
        return $"{id.Value} : {ints}";
    }

    [Production("ints : INT*")]
    public string ints(List<Token<TheLexer>> ints)
    {
        StringBuilder builder = new();
        builder.Append(string.Join("+", ints.Select(x => $"{x.Value}")));
        builder.Append(" = ");
        var sum = ints.Select(x => x.IntValue).Sum();
        builder.Append(sum);
        return builder.ToString();
    }
}

public enum TheLexer
{
    [AlphaId]
    ID,
    
    [Int]
    INT,
    
    [Sugar(":")]
    COLON,
}