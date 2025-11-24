using csly.models;

namespace extending;

public enum Toky
{
    [AlphaId] ID,
    [Keyword("A")] A,
    [Keyword("B")] B,
    [Keyword("C")] C,
    [Keyword("D")] D,
    [Keyword("X")] X,
    [Keyword("Y")] Y,
    [Keyword("Z")] Z
}




[ParserGenerator]
public partial class Tokynou : AbstractParserGenerator<Toky, ExtParser, string>
{
}