
using csly.manyParser.parser1.models;

namespace manyParser;

[ParserGenerator]
public partial class Parser1Generator : AbstractParserGenerator<Token1, Parser1 , string>
{
}

[ParserRoot("root1")]
public class Parser1
{

    [Production("root1 : ID1 ints1")]
    public string Root1(Token<Token1> id, string ids)
    {
        return $"{id.Value} : {ids}";
    }

    [Production("ints1: INT1*")]
    public string ints(List<Token<Token1>> ids)
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