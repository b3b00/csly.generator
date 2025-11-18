using csly.models;
using ebnf.grammar;
using System.Diagnostics;

namespace ebnf.grammar;

[ParserGenerator]

public partial class Ebnf : AbstractParserGenerator<EbnfTokenGeneric, RuleParser, GrammarNode>
{

}
