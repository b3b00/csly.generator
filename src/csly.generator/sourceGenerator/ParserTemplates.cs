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
    TerminalParserTemplate,
    ZeroOrMoreParserTemplate,
    OneOrMoreParserTemplate,
    OneOrMoreClauseTemplate,
    ZeroOrMoreClauseTemplate

}

internal enum VisitorTemplates
{
    CallVisitNonTerminalTemplate,
    CallVisitTerminalTemplate,
    CallVisitRuleTemplate,
    NonTerminalVisitorTemplate,
    RuleVisitorTemplate,
    VisitorTemplate,
    CallVisitZeroOrMoreTemplate,
    ZeroOrMoreVisitorTemplate
}   
