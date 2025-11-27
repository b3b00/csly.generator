

using ebnf.grammar;
using System;
using csly.ebnf.models;
using System.Collections.Generic;

    using System.Linq;

namespace ebnf.grammar
{

    public class RuleParserMain
    {

        private readonly RuleParser _instance;

        public RuleParserMain(RuleParser instance)
        {
            _instance = instance;
        }

        public ParseResult<EbnfTokenGeneric, GrammarNode> Parse(string source)
        {
            Console.WriteLine($"Parsing rule {source}");
            // lexing
            StaticEbnfTokenGeneric scanner = new StaticEbnfTokenGeneric();
            Console.WriteLine("\t1. Lexing...");
            var lexerResult = scanner.Scan(source.AsSpan());

            if (lexerResult.IsError)
            {
                return new ParseResult<EbnfTokenGeneric, GrammarNode>(lexerResult.Error);
            }

            // parsing
            var parser = new StaticRuleParser();
            Console.WriteLine("\t2. Anlysing...");
            var result = parser.ParseNonTerminal_rule(lexerResult.Tokens, 0);
            if (result.IsOk)
            {

                // visiting
                var visitor = new RuleParserVisitor(_instance);
                var output = visitor.Visitrule(result.Root as SyntaxNode<EbnfTokenGeneric, GrammarNode>);
                return new ParseResult<EbnfTokenGeneric, GrammarNode>(output, result.Root);
            }
            else
            {
                return new ParseResult<EbnfTokenGeneric, GrammarNode>(result.Errors.Cast<ParseError>().ToList());
            }
        }

    }
}


