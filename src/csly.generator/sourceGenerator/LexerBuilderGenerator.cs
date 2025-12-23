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

    private string _assemblyName;

    protected int upToCounter;

    public void AnalyseLexer(EnumDeclarationSyntax enumDeclarationSyntax,
        Dictionary<string, SyntaxNode> declarationsByName)
    {
        string name = enumDeclarationSyntax.Identifier.ToString();
        _templateEngine = new TemplateEngine(name, "", "", _staticLexerBuilder.NameSpace);

        LexerSyntaxWalker walker = new(name, declarationsByName, this, _staticLexerBuilder);
        walker.Visit(enumDeclarationSyntax);

    }

    public List<(string mode, string lexer, string fsm)> GenerateSubLexers()
    {
        // TODO : support multiple modes
        var fsms = GenerateFsms();
        var subLexers = new List<(string mode, string lexer, string fsm)>();
        foreach (var mode in fsms)
        {
            string lexer = Generate(mode.Key, mode.Value);
            var dump = new StringBuilder();
            dump.AppendLine("********************************");
            dump.AppendLine($"*** Mode: >>{mode.Key}<<");
            dump.AppendLine("********************************");
            dump.AppendLine();
            dump.Append(fsms.Values.ToString());

            subLexers.Add((mode.Key, lexer, mode.Value.ToString()));
        }
        return subLexers;
    }



    public const string startState = "start";
    public const string inIntState = "InInt";
    public const string inDoubleState = "InDouble";
    public const string inIdentifierState = "InIdentifier";
    public const string inStringState = "InString";
    public const string inEscapedStringState = "InEscapedString";



    public const int start = -1;

    public LexerBuilderGenerator(StaticLexerBuilder staticLexerBuilder, string? assemblyName)
    {
        _staticLexerBuilder = staticLexerBuilder;
        _assemblyName = assemblyName;
    }

    private void Log(string message)
    {
        Console.WriteLine($"[LexerBuilderGenerator] {message}");
    }

    public Dictionary<string, Fsm> GenerateFsms()
    {
        Dictionary<string, Fsm> fsms = new Dictionary<string, Fsm>();

        var modes = _staticLexerBuilder.Lexemes.SelectMany(lexem =>
        {
            if (lexem.Modes != null)
            {
                return lexem.Modes;
            }
            else
            {
                return new List<string>() { "default" };
            }
        }).Distinct();
        foreach (var mode in modes)
        {
            var lexemsInMode = _staticLexerBuilder.Lexemes.Where(lexem => lexem.Modes.Contains(mode)).ToList();
Log("Generating FSM for mode " + mode);
            var subFfsm = GenerateFSM(lexemsInMode);
            fsms.Add(mode, subFfsm);
        }

        return fsms;
    }

    public Fsm GenerateFSM(List<model.lexer.Lexeme> lexemsInMode)
    {

        Fsm fsm = new();
        foreach (var lexem in lexemsInMode)
        {
            Log("\nProcessing lexeme: " + lexem.Type + " -> " + lexem.Name);
            fsm.GoTo(startState);
            switch (lexem.Type)
            {
                case model.lexer.GenericToken.UpTo:
                    { 
                        
                        var exceptions = lexem.Args.Select(x => x.TrimQuotes()).ToList();
                        exceptions = exceptions.Where(x => !string.IsNullOrEmpty(x)).ToList();
                        exceptions = exceptions.Distinct().ToList();
                        exceptions = exceptions.Select(x => x.TrimQuotes()).ToList();
                        Log("  Generating UpTo lexeme with exceptions: " + string.Join(", ", exceptions));
                        Func<int, int, string> GetEndLabel = (int exception, int exceptionIndex) =>
                        {
                            if (exception < 0 || exceptionIndex < 0)
                            {
                                return $"in_upto_text_{upToCounter}";
                            }

                            return $"in_upto_{exception}_{exceptionIndex}_{upToCounter}";
                        };

                        var upToChars0 = new string(exceptions.Select(x => x.First()).Distinct().ToArray());
                        string startupTo = GetEndLabel(-1, -1);
                        // allow move with any char different from exceptions' first char
                        fsm.ExceptTransition(upToChars0);
                        fsm.Mark(startupTo);
                        fsm.End(lexem);
                        // loop if no exception char found
                        fsm.ExceptTransitionTo(startupTo, upToChars0);

                        // for each exception, create a path
                        for (int exceptionIndex = 0; exceptionIndex < exceptions.Count; exceptionIndex++)
                        {
                            var exception = exceptions[exceptionIndex];
                            fsm.GoTo(startupTo);
                            
                            
                            // match rest of exception chars
                            for (int i = 0; i < exception.Length-1; i++)
                            {
                                var current = fsm.GetCurrentState();
                                fsm.ExceptTransitionTo(startupTo, exception[i].ToString());
                                fsm.GoTo(current.Id);
                                var matchingState = GetEndLabel(exceptionIndex, i);
                                fsm.Transition(exception[i]);
                                fsm.Mark(matchingState);                                
                            }
                            
                            // if last char is different then go back to upto
                            fsm.ExceptTransitionTo(startupTo, exception[exception.Length-1].ToString());
                            
                        }
                        upToCounter++;
                        break;
                    }
                case model.lexer.GenericToken.SugarToken:
                    {
                        fsm.ConstantTransition(lexem.Arg0);
                        fsm.End(lexem);
                        if (lexem.IsPop)
                        {
                            fsm.Pop();
                        }
                        if (lexem.IsPush)
                        {
                            fsm.PushTo(lexem.PushTarget);
                        }
                        break;
                    }
                case model.lexer.GenericToken.Int:
                    {
                        var next = fsm.RangeTransition('0', '9');
                        fsm.Mark("InInt");
                        fsm.End(lexem);
                        fsm.RangeTransitionTo("InInt", '0', '9');
                        fsm.End(lexem);
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
                        fsm.End(lexem);
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
                return new Token<{_staticLexerBuilder.LexerName}>(keywordToken, match.Value, match.Position) {{
                    IsPop = match.IsPop,
                    PushTarget = match.PushTarget
                }};
            }}
            return new Token<{_staticLexerBuilder.LexerName}>(match.Token, match.Value, match.Position) {{
                    IsPop = match.IsPop,
                    PushTarget = match.PushTarget
                }};
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

                        for (var i = 0; i < startPattern.Count(); i++)
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
                            fsm.End(lexem);
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
                        char delim = '"';
                        char escape = '\\';
                        if (lexem.Args.Length == 2)
                        {
                            delim = lexem.Args[0].TrimQuotes()[0];
                            escape = lexem.Args[1].TrimQuotes()[0];
                        }

                        fsm.Transition(delim);
                        fsm.Mark(inStringState);
                        fsm.ExceptTransitionTo(inStringState, $"{delim}{escape}");
                        fsm.Transition(escape);
                        fsm.Mark(inEscapedStringState);
                        fsm.AnyTransitionTo(inStringState);
                        fsm.Transition(delim);

                        fsm.End(lexem);
                        break;
                    }
                case model.lexer.GenericToken.Comment:
                    {

                        fsm.GoTo(startState);
                        if (lexem.Args.Length == 2)
                        {
                            var startComment = lexem.Args[0].TrimQuotes();
                            fsm.ConstantTransition(startComment);
                            fsm.End(lexem, isMultiLineComment: true);
                            fsm.GetCurrentState().MultiLineCommentEndString = lexem.Args[1].TrimQuotes();
                        }
                        else if (lexem.Args.Length == 1)
                        {
                            var lineComment = lexem.Args[0].TrimQuotes();
                            fsm.ConstantTransition(lineComment);
                            fsm.End(lexem, isSingleLineComment: true);
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


    public string Generate(string mode, Fsm fsm)
    {
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
        var factories = string.Join("\n", fsm.Factories.Select(kvp =>
        {
            return $@"_tokenFactories.Add({_staticLexerBuilder.LexerName}.{kvp.Key},{kvp.Value});";
        }));

        return _templateEngine.ApplyTemplate(nameof(LexerTemplates.FsmTemplate), additional: new Dictionary<string, string>()
        {
            {"KEYWORDS", keywords },
            {"EPSILON_STATES", string.Join(", ", fsm.GetEpsilonStates().Select(x => x.ToString())) } ,
            {"STATE_TOKENS", string.Join(",\n", fsm.States.Where(s => s.TokenName != null).Select(s => $@"{{ {s.Id}, {_staticLexerBuilder.LexerName}.{s.TokenName} }}")) },
            {"EXPLICIT_KEYWORDS", explicitKeywords },
            {"FACTORIES", factories },
            { "STATES", statesCode },
            { "MODE", mode },
            { "STATE_CALLS", statesCall },
            {"ASSEMBLY", _assemblyName}
        });
    }

    public string Generate(Fsm fsm, State state)
    {
        StringBuilder sb = new StringBuilder();
        var transitions = fsm.GetTransitions(state.Id);

        string transitionsCode = "";
        if (!fsm.IsEpsilonState(state.Id))
        {
            transitionsCode = "char ch = GetChar(source, position);";
        }

        transitionsCode += string.Join("\n", transitions.Select(transition =>
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
                    {"IS_GOING_TO_END", targetState.IsEnd.ToString().ToLower() } 
                    //{ "ENDING", endingTransition }
                });

        }));

        var explicitMatch = $"new FsmMatch<{_staticLexerBuilder.LexerName} > (memory, _startPosition);";
        var match = $@"new FsmMatch<{_staticLexerBuilder.LexerName} > ({_staticLexerBuilder.LexerName}.{state.TokenName}, memory, _startPosition) 
    {{ 
        IsPop = {state.IsPop.ToString().ToLower()},
        PushTarget = ""{state.PushTarget}""
    }}";
        var comment = state.IsSingleLineComment ? "_currentMatch.IsSingleLineComment = true;" :
                      state.IsMultiLineComment ? @$"_currentMatch.IsMultiLineComment = true;
_currentMatch.MultiLineCommentEndDelimiter = ""{state.MultiLineCommentEndString}"";" :
                      "";
        var ending = state.IsEnd ?
            @$"    var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
        var memory = new ReadOnlyMemory<char>(sliced.ToArray());
        _currentMatch = {(state.IsExplicitEnd ? explicitMatch : match)} ;
        {comment}"
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