using System.ComponentModel.Design.Serialization;
using csly.jsonparser.jsonparser.models;
using jsonparser.JsonModel;

namespace jsonparser.JsonModel
{
}

namespace json
{
    [ParserGenerator]
    public partial class JSONParserGenerator : AbstractParserGenerator<JsonTokenGeneric, JsonParser , JSon>
    {
    }
}