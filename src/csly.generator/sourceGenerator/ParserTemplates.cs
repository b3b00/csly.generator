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
    NonTerminalClauseForManyTemplate,
    TerminalClauseForManyTemplate,
    ManyClauseTemplate
}

internal enum VisitorTemplates
{
    CallVisitNonTerminalTemplate,
    CallVisitTerminalTemplate,
    CallVisitRuleTemplate,
    NonTerminalVisitorTemplate,
    RuleVisitorTemplate,
    VisitorTemplate,
    CallVisitManyTemplate,
    ZeroOrMoreVisitorTemplate,
    OneOrMoreVisitorTemplate
}   
