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
        InChar
};

internal class StaticLexerGenerator {
    private readonly StaticLexerBuilder _lexerBuilder;

    private readonly TemplateEngine _templateEngine;

    private readonly List<States> _states = new List<States>();

    private readonly List<Lexeme> _longLexemes = new List<Lexeme>();
    public StaticLexerGenerator(StaticLexerBuilder lexerBuilder)
    {
        _lexerBuilder = lexerBuilder;
        _templateEngine = new TemplateEngine(_lexerBuilder.LexerName, "", "");
    }

    public string Generate()
    {
        var starts = _lexerBuilder.Lexemes.Select(lexeme => GenerateStart(lexeme)).Where(content => content != null).ToList();

        var others = _lexerBuilder.Lexemes.Select(lexeme => GenerateOther(lexeme)).Where(content => content != null).ToList();
        var startsIf = string.Join("else ", starts);
        var othersIf = string.Join("\n", others);
        var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.LexerTemplate),  additional: new Dictionary<string, string>()
        {
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
            var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.SugarTemplate), lexeme.Name, additional : new Dictionary<string, string>()
            {
                {"CHAR", lexeme.Args.First().Trim(new[]{'"'}) }
            });
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
            _states.Add(States.InIdentifier);
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
            _states.Add(States.InInt);
            return content;
        }
        return null;
    }

    public string GenerateOther(Lexeme lexeme)
    {
        if (lexeme.Type == GenericToken.Identifier)
        {
            var followPatterns = ParseIdentifierPattern(lexeme.Args[2]).ToList();
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
            return content;
        }
        if (lexeme.Type == GenericToken.Int)
        {
            var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.IntTemplate), lexeme.Name);
            return content;
        }
        return null;
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
