
using csly.models;
using jsonparser.JsonModel;

namespace jsonparser;

[ParserGenerator]
public partial class JSON : AbstractParserGenerator<JsonTokenGeneric, EbnfJsonGenericParser, JSon>
{
}