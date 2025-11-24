using csly.models;

namespace extending;

public enum Toky
{
    [AlphaId] ID,
    [Keyword("A")] A,
    [Keyword("B")] B,
    [Keyword("C")] C
}




[ParserGenerator]
public partial class Tokynou : AbstractParserGenerator<Toky, ExtParser, string>
{
}