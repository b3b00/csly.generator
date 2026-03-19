using csly.whiley.whileparsergeneric.models;
using csly.whileLang.model;



namespace csly.whileLang
{
    [ParserGenerator]
    public partial class While: AbstractParserGenerator<WhileTokenGeneric,WhileParserGeneric, WhileAST>
    {
    }
}
