using csly.models;
using ebnf.grammar;

namespace ebnf;

[ParserGenerator]

public class Ebnf : AbstractParserGenerator<EbnfToken, RuleParser, GrammarNode>
{

}