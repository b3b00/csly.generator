using csly.ebnf.models;
using csly.generator.sourceGenerator.fsm;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace csly.generator.sourceGenerator;

internal class LexerBuilderGenerator
{

    public List<string> Tokens { get; set; } = new List<string>();

    TemplateEngine _templateEngine;

    private StaticLexerBuilder _staticLexerBuilder;



    public void AnalyseLexer(EnumDeclarationSyntax enumDeclarationSyntax, 
        Dictionary<string, SyntaxNode> declarationsByName)
    {
        string name = enumDeclarationSyntax.Identifier.ToString();
        _templateEngine = new TemplateEngine(name, "", "", _staticLexerBuilder.NameSpace);

        LexerSyntaxWalker walker = new(name, declarationsByName, this, _staticLexerBuilder);
        walker.Visit(enumDeclarationSyntax);

    }

    public string GenerateLexer()
    {       

        var fsm = GenerateFSM();

        //string dump = fsm.ToString();
        //System.IO.File.WriteAllText($"C:\\tmp\\generation\\{name}_fsm.txt", dump);
        return Generate(fsm);

    }


    public const string startState = "start";
    public const string inIntState = "InInt";
    public const string inDoubleState = "InDouble";
    public const string inIdentifierState = "InIdentifier";
    public const string inStringState = "InString";
    public const string inEscapedStringState = "InEscapedString";



    public const int start = -1;

    public LexerBuilderGenerator(StaticLexerBuilder staticLexerBuilder)
    {
        _staticLexerBuilder = staticLexerBuilder;
        
    }

    public Fsm GenerateFSM()
    {

        Fsm fsm = new();
        foreach(var lexem in _staticLexerBuilder.Lexemes)
        {
            fsm.GoTo(startState);
            switch (lexem.Type)
            {
                case model.lexer.GenericToken.SugarToken:
                    {
                        fsm.ConstantTransition(lexem.Arg0);
                        fsm.End(lexem.Name, lexem.IsExplicit);
                        break;
                    }
                case model.lexer.GenericToken.Int:
                    {
                        var next = fsm.RangeTransition('0', '9');
                        fsm.Mark("InInt");
                        fsm.End(lexem.Name);
                        fsm.RangeTransitionTo("InInt", '0', '9');
                        fsm.End(lexem.Name);
                        break;
                    }
                case model.lexer.GenericToken.Double:
                    {
                        if (fsm.GetState(inIntState) == null)
                        {
                            fsm.GoTo(startState);
                            var next = fsm.RangeTransition('0', '9');
                            fsm.Mark(inIntState);
                            fsm.RangeTransitionTo(inIntState, '0', '9');
                        }
                        fsm.GoTo(inIntState);
                        fsm.Transition('.');
                        fsm.Mark(inDoubleState);
                        fsm.RangeTransitionTo(inDoubleState, '0', '9');
                        fsm.End(lexem.Name);
                        break;
                    }
                case model.lexer.GenericToken.KeyWord:
                    {
                        if (lexem.IsExplicit)
                        {
                            fsm.AddExplicitKeyword(lexem.Arg0);
                        }
                        else
                        {
                            fsm.AddKeyword(lexem.Arg0, lexem.Name);
                        }
                        break;
                    }
                case model.lexer.GenericToken.Identifier:
                    {
                        var startPattern = lexem.IdentifierStartPatterns();
                        fsm.AddFactory(lexem.Name, @$"match =>
        {{
            if (_keywords.TryGetValue(match.Value.ToString(), out var keywordToken))
            {{
                return new Token<{_staticLexerBuilder.LexerName}>(keywordToken, match.Value, match.Position);
            }}
            return new Token<{_staticLexerBuilder.LexerName}>(match.Token, match.Value, match.Position);
        }}");

                        var cond = string.Join(" || ", startPattern.Select(pattern =>
                        {
                            if (pattern.Length == 2)
                            {
                                return $"(currentChar >= '{pattern[0]}' && currentChar <= '{pattern[1]}')";
                            }
                            else
                            {
                                return $"(currentChar == '{pattern[0]}')";
                            }
                        }));

                        for ( var i = 0; i < startPattern.Count(); i++)
                        {
                            fsm.GoTo(startState);
                            var pattern = startPattern.ElementAt(i);
                            if (pattern.Length == 1)
                            {   
                                if (i == 0) 
                                    fsm.Transition(pattern[0]);
                                else
                                    fsm.TransitionTo(inIdentifierState, pattern[0]);
                            }
                            else if (pattern.Length == 2)
                            {
                                if (i == 0)
                                    fsm.RangeTransition(pattern[0], pattern[1]);
                                else
                                    fsm.RangeTransitionTo(inIdentifierState, pattern[0], pattern[1]);
                            }
                            fsm.Mark("InIdentifier");
                            fsm.End(lexem.Name);
                        }
                        var tailPatterns = lexem.IdentifierTailPatterns();
                        for (var i = 0; i < startPattern.Count(); i++)
                        {
                            fsm.GoTo(inIdentifierState);
                            var pattern = startPattern.ElementAt(i);
                            if (pattern.Length == 1)
                            {
                                fsm.TransitionTo(inIdentifierState, pattern[0]);
                            }
                            else if (pattern.Length == 2)
                            {
                                fsm.RangeTransitionTo(inIdentifierState, pattern[0], pattern[1]);
                            }
                        }
                        break;
                    }
                    case model.lexer.GenericToken.String:
                    {
                        char delim  = '"';
                        char escape = '\\';
                        if (lexem.Args.Length == 2)
                        {
                            delim = lexem.Args[0].Trim('"')[0];
                            escape = lexem.Args[1].Trim('"')[0];
                        }

                        fsm.Transition(delim);
                        fsm.Mark(inStringState);
                        fsm.ExceptTransitionTo(inStringState,$"{delim}{escape}");
                        fsm.Transition(escape);
                        fsm.Mark(inEscapedStringState);
                        fsm.AnyTransitionTo(inStringState);
                        fsm.Transition(delim);
                        
                        fsm.End(lexem.Name);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"FSM generation for lexeme type {lexem.Type} not implemented yet.");
                        // TODO : other lexeme types
                        break;
                    }
            }
        }
        return fsm;
    }


    public string Generate(Fsm fsm)
    {
        System.IO.File.WriteAllText($"C:\\tmp\\generation\\{_staticLexerBuilder.LexerName}_fsm.txt", fsm.ToString());

        var statesCode = string.Join("\n", fsm.States.Select(state => Generate(fsm, state)));

        var statesCall = string.Join("\n else ", fsm.States.Select(state =>
        {
            return _templateEngine.ApplyTemplate(nameof(LexerTemplates.StateCallTemplate), additional: new Dictionary<string, string>()
            {
                { "STATE", state.Id.ToString() },
                {"TOKEN", state.TokenName }
            });
        }));

        var keywords = string.Join(",\n", fsm.Keywords.Select(kvp =>
        {
            return $@"{{ ""{kvp.Key}"", {_staticLexerBuilder.LexerName}.{kvp.Value} }}";
        }));
        var explicitKeywords = string.Join(", ", fsm.ExplicitKeywords.Select(kw => $@"""{kw}"""));
        var factories = string.Join("\n",fsm.Factories.Select(kvp =>
        {
            return $@"_tokenFactories.Add({_staticLexerBuilder.LexerName}.{kvp.Key},{kvp.Value});";
        }));
        
        return _templateEngine.ApplyTemplate(nameof(LexerTemplates.FsmTemplate), additional: new Dictionary<string, string>()
        {
            {"KEYWORDS", keywords },
            {"EXPLICIT_KEYWORDS", explicitKeywords },
            {"FACTORIES", factories },
            { "STATES", statesCode },
            { "STATE_CALLS", statesCall }
        });
    }

    public string Generate(Fsm fsm, State state)
    {       
        StringBuilder sb = new StringBuilder();
        var transitions = fsm.GetTransitions(state.Id);

        var transitionsCode = string.Join("\n",transitions.Select(transition =>
        {
            

            var targetState = fsm.GetState(transition.TargetState);

            ;

            var explicitMatch = $"new FsmMatch<{_staticLexerBuilder.LexerName} > (memory, _startPosition);";
            var match = $"new FsmMatch<{_staticLexerBuilder.LexerName} > ({_staticLexerBuilder.LexerName}.{targetState.TokenName}, memory, _startPosition);";

        var endingTransition = state.IsEnd ? 
            @$"    var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
        var memory = new ReadOnlyMemory<char>(sliced.ToArray());
        _currentMatch = {(targetState.IsExplicitEnd ? explicitMatch : match)};"
        :
        "";

            
            return _templateEngine.ApplyTemplate(nameof(LexerTemplates.TransitionTemplate), additional: new Dictionary<string, string>()
                {
                    { "CURRENT_STATE", state.Id.ToString() },
                    { "CONDITION", transition.StringCondition },
                    { "NEW_STATE", targetState.Id.ToString() },
                    {"TOKEN_NAME", targetState.TokenName ?? "null" },
                    //{ "ENDING", endingTransition }
                });
            
        }));

        var explicitMatch = $"new FsmMatch<{_staticLexerBuilder.LexerName} > (memory, _startPosition);";
        var match = $"new FsmMatch<{_staticLexerBuilder.LexerName} > ({_staticLexerBuilder.LexerName}.{state.TokenName}, memory, _startPosition);";

        var ending = state.IsEnd ?
            @$"    var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
        var memory = new ReadOnlyMemory<char>(sliced.ToArray());
        _currentMatch = {(state.IsExplicitEnd ? explicitMatch : match)} ;"
        :
        "";

        return _templateEngine.ApplyTemplate(nameof(LexerTemplates.StateTemplate), additional: new Dictionary<string, string>()
        {
            { "STATE_ID", state.Id.ToString() },
            { "TRANSITIONS", transitionsCode },
            { "IS_END", state.IsEnd.ToString().ToLower() },
            {"ENDING", ending }
        });

        return sb.ToString();

    }
}