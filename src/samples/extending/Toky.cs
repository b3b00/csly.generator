using csly.models;

namespace extending;

public enum Toky
{
    [AlphaId] ID,
    [Keyword("A")] A,
    [Keyword("AA")] AA,
    [Keyword("B")] B,
    [Keyword("C")] C,
    [Keyword("D")] D,
    [Keyword("X")] X,
    [Keyword("Y")] Y,
    [Keyword("Z")] Z,
    [Sugar("+")] PLUS,
    [Sugar("++")] INC,
    [Sugar("=")] ASSIGN,
    [Sugar("==")] EQUALS,
    [Int] INT,
    [Double] DOUBLE,
    [String] STRING_1,
    [String("'", "\\")] STRING_2,
    [Push("brackets")]
    [Sugar(">")] GT,
    [Mode("brackets")]
    [Sugar(".")] DOT,
    [Pop()]
    [Sugar("<")] LT,
}




[ParserGenerator]
public partial class Tokynou : AbstractParserGenerator<Toky, ExtParser, string>
{
}