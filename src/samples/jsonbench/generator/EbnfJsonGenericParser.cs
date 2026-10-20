using BenchCslies.parsers.JsonModel;
using csly.ebnf.models;
using sly.parser.generator;

namespace BenchCslies.parsers.generator
{
    
    [ParserGenerator]
    public partial class TheParserGenerator : AbstractParserGenerator<GeneratedCslyJsonTokenGeneric, GeneratedEbnfJsonGenericParser, JSon>{
    
    }
    
    [sly.parser.generator.BroadenTokenWindow]
    [sly.parser.generator.ParserRoot("root")]
    [FirstDerivation]
    public class GeneratedEbnfJsonGenericParser
    {
        #region root

        [sly.parser.generator.Production("root : value")]
        public JSon Root(JSon value)
        {
            return value;
        }

        #endregion

        #region VALUE

        [sly.parser.generator.Production("value : STRING")]
        public JSon StringValue(sly.lexer.Token<GeneratedCslyJsonTokenGeneric> stringToken)
        {
            return new JValue(stringToken.StringWithoutQuotes);
        }

        [sly.parser.generator.Production("value : INT")]
        public JSon IntValue(sly.lexer.Token<GeneratedCslyJsonTokenGeneric> intToken)
        {
            return new JValue(intToken.IntValue);
        }

        [sly.parser.generator.Production("value : DOUBLE")]
        public JSon DoubleValue(sly.lexer.Token<GeneratedCslyJsonTokenGeneric> doubleToken)
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

            return new JValue(dbl);
        }

        [sly.parser.generator.Production("value : BOOLEAN")]
        public JSon BooleanValue(sly.lexer.Token<GeneratedCslyJsonTokenGeneric> boolToken)
        {
            return new JValue(bool.Parse(boolToken.Value));
        }

        [sly.parser.generator.Production("value : NULL[d]")]
        public JSon NullValue()
        {
            return new JNull();
        }

        [sly.parser.generator.Production("value : object")]
        public JSon ObjectValue(JSon value)
        {
            return value;
        }

        [sly.parser.generator.Production("value: list")]
        public JSon ListValue(JList list)
        {
            return list;
        }

        #endregion

        #region OBJECT

        [sly.parser.generator.Production("object: ACCG[d] ACCD[d]")]
        public JSon EmptyObjectValue()
        {
            return new JObject();
        }

        [sly.parser.generator.Production("object: ACCG[d] members ACCD[d]")]
        public JSon AttributesObjectValue(JObject members)
        {
            return members;
        }

        #endregion

        #region LIST

        [sly.parser.generator.Production("list: CROG[d] CROD[d]")]
        public JSon EmptyList()
        {
            return new JList();
        }

        [sly.parser.generator.Production("list: CROG[d] listElements CROD[d]")]
        public JSon List(JList elements)
        {
            return elements;
        }


        [sly.parser.generator.Production("listElements: value additionalValue*")]
        public JSon listElements(JSon head, List<JSon> tail)
        {
            var values = new JList(head);
            values.AddRange(tail);
            return values;
        }

        [sly.parser.generator.Production("additionalValue: COMMA value")]
        public JSon ListElementsOne(sly.lexer.Token<GeneratedCslyJsonTokenGeneric> discardedComma, JSon value)
        {
            return value;
        }

        #endregion

        #region PROPERTIES

        [sly.parser.generator.Production("members: property additionalProperty*")]
        public JSon Members(JObject head, List<JSon> tail)
        {
            var value = new JObject();
            value.Merge(head);
            foreach (var p in tail) value.Merge((JObject) p);
            return value;
        }

        [sly.parser.generator.Production("additionalProperty : COMMA[d] property")]
        public JSon property( JObject property)
        {
            return property;
        }

        [sly.parser.generator.Production("property: STRING COLON[d] value")]
        public JSon property(sly.lexer.Token<GeneratedCslyJsonTokenGeneric> key, JSon value)
        {
            return new JObject(key.StringWithoutQuotes, value);
        }

        #endregion
    }
    
    
}