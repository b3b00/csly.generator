
using csly.manyParser.models;

namespace manyParser;

[ParserGenerator]
public partial class Parser2Generator : AbstractParserGenerator<Token2, Parser2 , string>
{
}

[ParserRoot("root2")]
public class Parser2
{

    [Production("root2 : ID ints2")]
    public string Root1(Token<Token1> id, string ids)
    {
        return $"{id.Value} : {ids}";
    }

    [Production("ids: INT*")]
    public string ints(List<Token<Token2>> ids)
    {
        return string.Join(" ", ids.Select(i => i.Value));
    }
    
}

public enum Token1
{
    
    [AlphaId]
    ID1,
    
    [Int]
    INT1
}