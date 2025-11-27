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
    OptionParserTemplate,
    TerminalClauseForManyTemplate,
    ManyClauseTemplate,
    OptionClauseTemplate,
    TerminalClauseForOptionTemplate,
    NonTerminalClauseForOptionTemplate,
    ChoiceParserTemplate,
    ChoiceClauseTemplate,
    TerminalClauseInChoiceTemplate,
    NonTerminalClauseInChoiceTemplate
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
    OneOrMoreVisitorTemplate,
    CallVisitOptionTemplate,
    OptionVisitorTemplate,
    CallVisitChoiceTemplate,
    TerminalChoiceVisitorTemplate,
    NonTerminalChoiceVisitorTemplate,
    NonTerminalChoiceVisitorCall
}   
