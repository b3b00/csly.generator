
using json;

using NFluent;
using JObject = jsonparser.JsonModel.JObject;
using JValue = jsonparser.JsonModel.JValue;
using JList = jsonparser.JsonModel.JList;
using JNull = jsonparser.JsonModel.JNull;



namespace ParserTests.samples;

[CollectionDefinition("jsonParser", DisableParallelization = true)]
public class JsonParserTests
{



    public JSONParserMain BuildParser()
    {
        var parser = new JSONParserMain(new JsonParser(), useMemoization: true);
        return parser;
    }

    [Fact]
    public void Parse_Object()
    {
        var parser = BuildParser();
        var json = @"{
    ""string"": ""value"",
    ""int"":42
}";
        var result =  parser.Parse(json);
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result).IsInstanceOf<JObject>();
        var obj = (JObject)result.Result;
        Check.That(obj["string"]).IsNotNull();
        Check.That(obj["string"]).IsInstanceOf<JValue>();
        var stringProperty = obj["string"] as JValue;
        Check.That(stringProperty).IsNotNull();
        Check.That(stringProperty.GetValue<string>()).IsEqualTo("value");
        Check.That(obj["int"]).IsNotNull();
        Check.That(obj["int"]).IsNotNull();
        Check.That(obj["int"]).IsInstanceOf<JValue>();
        var intProperty = obj["int"] as JValue;
        Check.That(intProperty).IsNotNull();
        Check.That(intProperty.GetValue<int>()).IsEqualTo(42);
    }
    
    [Fact]
    public void Parse_Array()
    {
        var parser = BuildParser();
        var json = @"[1,2,3,4,5]";
        var result =  parser.Parse(json);
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result).IsInstanceOf<JList>();
        var array = (JList)result.Result;
        Check.That(array).IsNotNull();
        Check.That(array.Items).CountIs(5);
        Check.That(array.Items[0]).IsInstanceOf<JValue>();
        Check.That((array.Items[0]as JValue).GetValue<int>()).IsEqualTo(1);
        Check.That(array.Items[1]).IsInstanceOf<JValue>();
        Check.That((array.Items[1]as JValue).GetValue<int>()).IsEqualTo(2);
        Check.That(array.Items[2]).IsInstanceOf<JValue>();
        Check.That((array.Items[2] as JValue).GetValue<int>()).IsEqualTo(3);
        Check.That(array.Items[3]).IsInstanceOf<JValue>();
        Check.That((array.Items[3] as JValue).GetValue<int>()).IsEqualTo(4);
        Check.That(array.Items[4]).IsInstanceOf<JValue>();
        Check.That((array.Items[4] as JValue).GetValue<int>()).IsEqualTo(5);
    }

    [Fact]
    public void parse_Null()
    {
        var parser = BuildParser();
        var json = @"null";
        var result =  parser.Parse(json);
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result).IsInstanceOf<JNull>();
    }
    
    [Fact]
    public void parse_True()
    {
        var parser = BuildParser();
        var json = @"true";
        var result =  parser.Parse(json);
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result).IsInstanceOf<JValue>();
        var value = (JValue)result.Result;
        Check.That(value.GetValue<bool>()).IsTrue();
    }
    
    [Fact]
    public void parse_False()
    {
        var parser = BuildParser();
        var json = @"false";
        var result =  parser.Parse(json);
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result).IsInstanceOf<JValue>();
        var value = (JValue)result.Result;
        Check.That(value.GetValue<bool>()).IsFalse();
    }


    
}
