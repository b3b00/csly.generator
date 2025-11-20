using System;
using System.Collections.Generic;
using System.Text;

namespace csly.generator.sourceGenerator
{
    using csly.generator.model.lexer;
    using csly.generator.sourceGenerator;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Collections.Generic;
    using System.Linq;


    internal enum States
    {
        Start,
        InIdentifier,
        InInt,
        InDouble,
        InString,
        InChar,
        Sugar
    };

    internal class StaticLexerGenerator
    {
        private readonly StaticLexerBuilder _lexerBuilder;

        private readonly TemplateEngine _templateEngine;

        private readonly List<string> _states = new List<string>();

        private readonly List<Lexeme> _longLexemes = new List<Lexeme>();
        public StaticLexerGenerator(StaticLexerBuilder lexerBuilder)
        {
            _lexerBuilder = lexerBuilder;
            _templateEngine = new TemplateEngine(_lexerBuilder.LexerName, "", "", _lexerBuilder.NameSpace);
        }

        public string Generate()
        {
            var starts = _lexerBuilder.Lexemes.Select(lexeme => GenerateStart(lexeme)).Where(content => content != null).ToList();
            var o = _lexerBuilder.Lexemes.SelectMany(lexeme => GenerateOther(lexeme));
            var others = o.Where(content => content != null).ToList();
            var startsIf = string.Join("else ", starts);
            var othersIf = string.Join("else ", others);
            var keywords = string.Join(",\n        ", _lexerBuilder.Lexemes
                .Where(lexeme => lexeme.Type == GenericToken.KeyWord)
                .Select(lexeme => $"{{ \"{lexeme.Name}\", ExpressionToken.{lexeme.Name} }}"));
            var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.LexerTemplate), additional: new Dictionary<string, string>()
        {
            { "KEYWORDS", keywords},
            {"START_RULES", startsIf },
            {"STATES", string.Join(", ",_states.Select(x => x.ToString())) },
            {"OTHER_STATES", othersIf }
        });

            var syntaxTree = CSharpSyntaxTree.ParseText(content);
            var root = syntaxTree.GetRoot();

            return root.ToString();
        }

        public string GenerateStart(Lexeme lexeme)
        {
            if (lexeme.Type == GenericToken.SugarToken)
            {
                string nextState = $"{nameof(States.Sugar)}_{lexeme.Name}_1";
                var currentChar = lexeme.Args.First().Trim(new[] { '"' })[0];
                string condition = $"currentChar == '{currentChar}'";
                var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.OtherStartTemplate), lexeme.Name,
                    additional: new Dictionary<string, string>()
            {
                        
                {"CHAR", lexeme.Args.First().Trim(new[]{'"'}) },
                        {"NEW_STATE",nextState },
                        {"LEXEME_CONDITION",condition },
                        {"PATTERN", lexeme.Args.First().Trim(new[]{'"'}) }
            });
                _states.Add(nextState);
                return content;
            }
            if (lexeme.Type == GenericToken.Identifier)
            {
                var startPatterns = ParseIdentifierPattern(lexeme.Args[1]).ToList();

                var cond = string.Join(" || ", startPatterns.Select(pattern =>
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
                var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.OtherStartTemplate), additional: new Dictionary<string, string>()
            {
                {"LEXEME_CONDITION", cond },
                {"NEW_STATE", States.InIdentifier.ToString() }
            });
                _longLexemes.Add(lexeme);
                _states.Add(States.InIdentifier.ToString());
                return content;
            }
            if (lexeme.Type == GenericToken.Int)
            {

                var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.OtherStartTemplate), additional: new Dictionary<string, string>()
            {
                {"LEXEME_CONDITION", "char.IsDigit(currentChar)" },
                {"NEW_STATE", States.InInt.ToString() }
            });
                _longLexemes.Add(lexeme);
                _states.Add(States.InInt.ToString());
                return content;
            }
            return null;
        }

        public List<string> GenerateOther(Lexeme lexeme)
        {
            if (lexeme.Type == GenericToken.SugarToken)
            {
                List<string> nextStates = new List<string>();

                var pattern = lexeme.Args.First().Trim(new[] { '"' });
                int last = 1;
                if (pattern.Length > 1)
                {

                    for (int i = 1; i < pattern.Length; i++)
                    {
                        last = i+1;
                        char currentChar = pattern[i];
                        string condition = $"(state == LexerStates.{nameof(States.Sugar)}_{lexeme.Name}_{i} && currentChar == '{currentChar}')";
                        string currentState = $"{nameof(States.Sugar)}_{lexeme.Name}_{i + 1}";
                        string nextState = currentState;
                        var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.NextSugarTemplate), lexeme.Name,
                            additional: new Dictionary<string, string>()
                    {
                        {"CHAR", pattern[i].ToString() },
                                {"NEW_STATE",nextState },
                                {"LEXEME_CONDITION",condition }
                    });
                        nextStates.Add(content);
                        _states.Add(currentState);
                    }
                }

                string conditionEnd = $"(state == LexerStates.{nameof(States.Sugar)}_{lexeme.Name}_{last} )";
                var endContent = _templateEngine.ApplyTemplate(nameof(LexerTemplates.SugarTemplate), lexeme.Name,
                    additional: new Dictionary<string, string>()
                    {
                        {"LEXEME_CONDITION", conditionEnd },
                        {"NEW_STATE","Start" },
                        {"PATTERN", pattern }
                    });
                nextStates.Add(endContent);

                return nextStates;
            }
            if (lexeme.Type == GenericToken.Identifier)
            {
                var followPatterns = ParseIdentifierPattern(lexeme.Args[1]).ToList();
                var cond = string.Join(" || ", followPatterns.Select(pattern =>
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

                var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.IdentifierTemplate), lexeme.Name, additional: new Dictionary<string, string>()
            {
                {"LEXEME_NAME", lexeme.Name },
                {"CONDITION", cond }
            });
                return new List<string>() { content };
            }
            if (lexeme.Type == GenericToken.Int)
            {
                var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.IntTemplate), lexeme.Name);
                return new List<string>() { content };
            }
            return new List<string>();
        }


        private static IEnumerable<char[]> ParseIdentifierPattern(string pattern)
        {
            var index = 0;
            while (index < pattern.Length)
            {
                if (index <= pattern.Length - 3 && pattern[index + 1] == '-')
                {
                    if (pattern[index] < pattern[index + 2])
                    {
                        yield return new[] { pattern[index], pattern[index + 2] };
                    }
                    else
                    {
                        yield return new[] { pattern[index + 2], pattern[index] };
                    }

                    index += 3;
                }
                else
                {
                    yield return new[] { pattern[index++] };
                }
            }
        }
    }
}
