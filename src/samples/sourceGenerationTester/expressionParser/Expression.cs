using csly.ebnf.models;

namespace sourceGenerationTester.expressionParser;

[ParserGenerator]
public partial class Expression : AbstractParserGenerator<ExpressionToken, ExpressionParser, double>
{

}