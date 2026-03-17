using csly.jsonparser.models;

namespace json;

public abstract class JSon
{
    public virtual bool IsObject { get; set; }
    public virtual bool IsList { get; set; }
    public virtual bool IsValue { get; set; }
    public virtual bool IsNull { get; set; }
        
}

[ParserGenerator]
public partial class JSON : AbstractParserGenerator<JsonTokenGeneric, JSONParser, JSon>
{
}