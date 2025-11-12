using System.Diagnostics.CodeAnalysis;
using System.Text;
using csly.generator.model.lexer;

namespace csly.generator.model.parser
{

    public static class StringExtensions
    {
        public static List<string> GetLines(this string text)
        {
            List<string> lines = new List<string>();
            using (var reader = new StringReader(text))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }
            return lines;
        }

        public static string Multiply(this string str, int n)
        {
            string result = "";
            for (int i = 0; i < n; i++)
            {
                result += str;
            }

            return result;
        }
    }
        
    
    public class UnexpectedTokenSyntaxError<T> : ParseError, IComparable where T : struct, Enum
    {
        private readonly string _i18N;

        private readonly Dictionary<T, Dictionary<string, string>> _labels = new Dictionary<T, Dictionary<string, string>>();
        public UnexpectedTokenSyntaxError(Token<T> unexpectedToken, Dictionary<T, Dictionary<string, string>> labels, string i18n=null, params LeadingToken<T>[] expectedTokens )
        {
            _labels = labels;
            _i18N = i18n;
            ErrorType = unexpectedToken.IsEOS ? ErrorType.UnexpectedEOS : ErrorType.UnexpectedToken;
            UnexpectedToken = unexpectedToken;
            if (expectedTokens != null)
            {
                ExpectedTokens = new List<LeadingToken<T>>();
                ExpectedTokens.AddRange(expectedTokens);
            }
            else
            {
                ExpectedTokens = null;
            }

        }

        public UnexpectedTokenSyntaxError(Token<T> unexpectedToken, string i18n = null, List<LeadingToken<T>> expectedTokens = null) 
        {
            _i18N = i18n;
            ErrorType = unexpectedToken.IsEOS ? ErrorType.UnexpectedEOS : ErrorType.UnexpectedToken;
            
            UnexpectedToken = unexpectedToken;
            if (expectedTokens != null)
            {
                ExpectedTokens = new List<LeadingToken<T>>();
                ExpectedTokens.AddRange(expectedTokens);
            }
            else
            {
                ExpectedTokens = null;
            }
        }


        public Token<T> UnexpectedToken { get; set; }

        public List<LeadingToken<T>> ExpectedTokens { get; set; }

        public override int Line
        {
            get
            {
                var l = UnexpectedToken?.Position?.Line;
                return l.HasValue ? l.Value : 1;
            }
        }

        public override int Column
        {
            get
            {
                var c = UnexpectedToken?.Position?.Column;
                return c.HasValue ? c.Value : 1;
            }
        }

        [ExcludeFromCodeCoverage] public override string ErrorMessage => "TODO ";
        

        private string GetMessageForExpectedToken(LeadingToken<T> expected)
        {
            var message = new StringBuilder();
            if (expected.IsExplicitToken)
            {
                message.Append(expected.ToString());
            }
            else
            {
                var lbl = expected.ToString();
                if (_labels.TryGetValue(expected.TokenId, out var labels) && labels.TryGetValue(_i18N, out var label))
                {
                    lbl = label;
                }

                message.Append(lbl);
            }

            message.Append(", ");
            return message.ToString();
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return ErrorMessage;
        }
        
    }
}