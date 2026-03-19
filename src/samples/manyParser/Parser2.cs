
using csly.manyParser.parser2.models;

namespace manyParser;

[ParserGenerator]
public partial class Parser2Generator : AbstractParserGenerator<Token2, Parser2 , string>
{
}

[ParserRoot("root2")]
public class Parser2
{

    [Production("root2 : ID2 ints2")]
    public string Root1(Token<Token2> id, string ids)
    {
        return $"{id.Value} : {ids}";
    }

    [Production("ints2: INT2*")]
    public string ints(List<Token<Token2>> ids)
    {
        return string.Join(" ", ids.Select(i => i.Value));
    }
    
}

public enum Token2
{
    
    [AlphaId]
    ID2,
    
    [Int]
    INT2
}