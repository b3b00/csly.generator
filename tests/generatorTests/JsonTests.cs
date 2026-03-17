using NFluent;
using json;
using Newtonsoft.Json.Linq;
using JObject = json.JObject;
using JValue = json.JValue;


namespace ParserTests.samples;

[CollectionDefinition("xml", DisableParallelization = true)]
public class JsonTests
{



    public JSONParserMain BuildParser()
    {
        JSONParser instance = new JSONParser();

        var parser = new JSONParserMain(instance);
        return parser;

    }


    [Fact]
    public void TestSimpleObject()
    {
        var parser = BuildParser();
        var result = parser.Parse(@"{ 
    ""string"":""value"",
    ""int"" : 42
}");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        JObject obj =  (JObject)result.Result;
        
        Check.That(obj["string"]).IsNotNull();
        var stringProperty = obj["string"] as JValue;
        Check.That(stringProperty).IsNotNull();
        var stringValue = stringProperty.GetValue<string>();
        Check.That(stringValue).IsNotNull();
        Check.That(stringValue).IsEqualTo("value");
        
        Check.That(obj["int"]).IsNotNull();
        var intProperty = obj["int"] as JValue;
        Check.That(intProperty).IsNotNull();
        var intValue = intProperty.GetValue<int>();
        Check.That(intValue).IsEqualTo(42);
        
    }

    [Fact]
    public void TestArray()
    {
        var parser = BuildParser();
        var result = parser.Parse(@"[1,2,3,4,5]");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        JList list = (JList)result.Result;
        Check.That(list).IsNotNull();
        Check.That(list.Count).IsEqualTo(5);
    }
}
