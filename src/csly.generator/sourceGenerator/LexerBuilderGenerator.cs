using csly.ebnf.models;
using csly.generator.sourceGenerator.fsm;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csly.generator.sourceGenerator;

internal class LexerBuilderGenerator
{

    public List<string> Tokens { get; set; } = new List<string>();

    TemplateEngine _templateEngine;

    private StaticLexerBuilder _staticLexerBuilder;

    public string GenerateLexer(EnumDeclarationSyntax enumDeclarationSyntax, string outputType,
        Dictionary<string, SyntaxNode> declarationsByName, StaticLexerBuilder staticLexerBuilder)
    {
        _staticLexerBuilder = staticLexerBuilder;
        string name = enumDeclarationSyntax.Identifier.ToString();

        _templateEngine = new TemplateEngine(name, "", "", staticLexerBuilder.NameSpace);

        LexerSyntaxWalker walker = new(name, declarationsByName, this, staticLexerBuilder);
        walker.Visit(enumDeclarationSyntax);

        var fsm = GenerateFSM(staticLexerBuilder);

        return Generate(fsm);

    }


    public const string startState = "start";
    public const string inIntState = "InInt";
    public const string inDoubleState = "InDouble";
    public const string inIdentifierState = "InIdentifier";



    public const int start = -1;
    
    public Fsm GenerateFSM(StaticLexerBuilder staticLexerBuilder)
    {

        Fsm fsm = new();
        foreach(var lexem in staticLexerBuilder.Lexemes)
        {
            fsm.GoTo(startState);
            switch (lexem.Type)
            {
                case model.lexer.GenericToken.SugarToken:
                    {
                        fsm.ConstantTransition(lexem.Arg0);
                        break;
                    }
                case model.lexer.GenericToken.Int:
                    {
                        var next = fsm.RangeTransition('0', '9');
                        fsm.Mark("InInt");
                        fsm.RangeTransitionTo("InInt", '0', '9');
                        break;
                    }
                case model.lexer.GenericToken.Double:
                    {
                        fsm.GoTo(inIntState);
                        fsm.Transition('.');
                        fsm.Mark(inDoubleState);
                        fsm.RangeTransitionTo(inDoubleState, '0', '9');
                        fsm.End(lexem.Name);
                        break;
                    }
                case model.lexer.GenericToken.Identifier:
                    {
                        var startPattern = lexem.IdentifierStartPatterns();


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
        var statesCode = string.Join("\n", fsm.States.Select(state => Generate(fsm, state)));

        var statesCall = string.Join("\n", fsm.States.Select(state =>
        {
            return _templateEngine.ApplyTemplate(nameof(LexerTemplates.StateCallTemplate), additional: new Dictionary<string, string>()
            {
                { "STATE", state.Id.ToString() },
                {"TOKEN", state.TokenName }
            });
        }));

        return _templateEngine.ApplyTemplate(nameof(LexerTemplates.FsmTemplate), additional: new Dictionary<string, string>()
        {
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

            var endingTransition = state.IsEnd ? 
            @$"    var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
        var memory = new ReadOnlyMemory<char>(sliced.ToArray());
        _currentMatch = new FsmMatch<{_staticLexerBuilder.LexerName}>({_staticLexerBuilder.LexerName}.{targetState.TokenName}, memory, _startPosition)  ;"
        :
        "";

            
            return _templateEngine.ApplyTemplate(nameof(LexerTemplates.TransitionTemplate), additional: new Dictionary<string, string>()
                {
                    { "CONDITION", transition.StringCondition },
                    { "NEW_STATE", targetState.Id.ToString() },
                    {"TOKEN_NAME", targetState.TokenName ?? "null" },
                    { "ENDING", endingTransition }
                });
            
        }));

        var ending = state.IsEnd ?
            @$"    var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
        var memory = new ReadOnlyMemory<char>(sliced.ToArray());
        _currentMatch = new FsmMatch<{_staticLexerBuilder.LexerName}>({_staticLexerBuilder.LexerName}.{state.TokenName}, memory, _startPosition)  ;"
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