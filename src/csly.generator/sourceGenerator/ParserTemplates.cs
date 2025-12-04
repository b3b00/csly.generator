using System;
using System.Collections.Generic;
using System.Text;

namespace csly.generator.sourceGenerator;

internal enum ParserTemplates
{
    ExplicitTerminalParserTemplate,
    HelpersTemplate,
    NonTerminalClauseTemplate,
    NonTerminalParserTemplate,
    ParserTemplate,
    RuleCallTemplate,
    RuleParserTemplate,
    TerminalClauseTemplate,
    ExplicitTerminalClauseTemplate,
    TerminalParserTemplate,
    ZeroOrMoreParserTemplate,
    OneOrMoreParserTemplate,
    NonTerminalClauseForManyTemplate,
    OptionParserTemplate,
    TerminalClauseForManyTemplate,
    ManyClauseTemplate,
    OptionClauseTemplate,
    TerminalClauseForOptionTemplate,
    NonTerminalClauseForOptionTemplate,
    ChoiceParserTemplate,
    ChoiceClauseTemplate,
    TerminalClauseInChoiceTemplate,
    NonTerminalClauseInChoiceTemplate,
    ExpressionInfixRuleParser,
    ExpressionPrefixRuleParser,
    ExpressionPostfixRuleParser
}


