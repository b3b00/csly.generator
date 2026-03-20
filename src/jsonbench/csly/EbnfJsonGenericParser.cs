using jsonparser;
using jsonparser.JsonModel;
using sly.lexer;
using sly.parser.generator;
using JList = benchgenerator.csly.JsonModel.JList;
using JNull = benchgenerator.csly.JsonModel.JNull;
using JObject = benchgenerator.csly.JsonModel.JObject;
using JSon = benchgenerator.csly.JsonModel.JSon;
using JValue = benchgenerator.csly.JsonModel.JValue;

namespace benchgenerator.csly
{
    using JNull = JsonModel.JNull;
    using JsonModel_JList = JsonModel.JList;
    using JsonModel_JObject = JsonModel.JObject;
    using JsonModel_JSon = JsonModel.JSon;
    using JsonModel_JValue = JsonModel.JValue;

    [BroadenTokenWindow]
    [ParserRoot("root")]
    public class EbnfJsonGenericParser
    {
        #region root

        [Production("root : value")]
        public JsonModel_JSon Root(JsonModel_JSon value)
        {
            return value;
        }

        #endregion

        #region VALUE

        [Production("value : STRING")]
        public JsonModel_JSon StringValue(Token<JsonTokenGeneric> stringToken)
        {
            return new JsonModel_JValue(stringToken.StringWithoutQuotes);
        }

        [Production("value : INT")]
        public JsonModel_JSon IntValue(Token<JsonTokenGeneric> intToken)
        {
            return new JsonModel_JValue(intToken.IntValue);
        }

        [Production("value : DOUBLE")]
        public JsonModel_JSon DoubleValue(Token<JsonTokenGeneric> doubleToken)
        {
            double dbl;
            try
            {
                var doubleParts = doubleToken.Value.Split('.');
                dbl = double.Parse(doubleParts[0]);
                if (doubleParts.Length > 1)
                {
                    var decimalPart = double.Parse(doubleParts[1]);
                    for (var i = 0; i < doubleParts[1].Length; i++) decimalPart = decimalPart / 10.0;
                    dbl += decimalPart;
                }
            }
            catch (Exception)
            {
                dbl = double.MinValue;
            }

            return new JsonModel_JValue(dbl);
        }

        [Production("value : BOOLEAN")]
        public JsonModel_JSon BooleanValue(Token<JsonTokenGeneric> boolToken)
        {
            return new JsonModel_JValue(bool.Parse(boolToken.Value));
        }

        [Production("value : NULL[d]")]
        public JsonModel_JSon NullValue()
        {
            return new JNull();
        }

        [Production("value : object")]
        public JsonModel_JSon ObjectValue(JsonModel_JSon value)
        {
            return value;
        }

        [Production("value: list")]
        public JsonModel_JSon ListValue(JsonModel_JList list)
        {
            return list;
        }

        #endregion

        #region OBJECT

        [Production("object: ACCG[d] ACCD[d]")]
        public JsonModel_JSon EmptyObjectValue()
        {
            return new JsonModel_JObject();
        }

        [Production("object: ACCG[d] members ACCD[d]")]
        public JsonModel_JSon AttributesObjectValue(JsonModel_JObject members)
        {
            return members;
        }

        #endregion

        #region LIST

        [Production("list: CROG[d] CROD[d]")]
        public JsonModel_JSon EmptyList()
        {
            return new JsonModel_JList();
        }

        [Production("list: CROG[d] listElements CROD[d]")]
        public JsonModel_JSon List(JsonModel_JList elements)
        {
            return elements;
        }


        [Production("listElements: value additionalValue*")]
        public JsonModel_JSon listElements(JsonModel_JSon head, List<JsonModel_JSon> tail)
        {
            var values = new JsonModel_JList(head);
            values.AddRange(tail);
            return values;
        }

        [Production("additionalValue: COMMA value")]
        public JsonModel_JSon ListElementsOne(Token<JsonTokenGeneric> discardedComma, JsonModel_JSon value)
        {
            return value;
        }

        #endregion

        #region PROPERTIES

        [Production("members: property additionalProperty*")]
        public JsonModel_JSon Members(JsonModel_JObject head, List<JsonModel_JSon> tail)
        {
            var value = new JsonModel_JObject();
            value.Merge(head);
            foreach (var p in tail) value.Merge((JsonModel_JObject) p);
            return value;
        }

        [Production("additionalProperty : COMMA[d] property")]
        public JsonModel_JSon property( JsonModel_JObject property)
        {
            return property;
        }

        [Production("property: STRING COLON[d] value")]
        public JsonModel_JSon property(Token<JsonTokenGeneric> key, JsonModel_JSon value)
        {
            return new JsonModel_JObject(key.StringWithoutQuotes, value);
        }

        #endregion
    }
    
     
}