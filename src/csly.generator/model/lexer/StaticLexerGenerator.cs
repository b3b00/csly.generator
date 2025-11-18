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
    InSugar
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
        List<string> starts = new();
        try
        {
            var s = _lexerBuilder.Lexemes.SelectMany(lexeme => GenerateStart(lexeme));
                starts = s.Where(content => content != null).ToList();
        }
        catch(System.Exception ex)
        {
            System.Console.WriteLine(ex.ToString());
        }

        var others = _lexerBuilder.Lexemes.SelectMany(lexeme => GenerateOther(lexeme)).Where(content => content != null).ToList();
        var startsIf = string.Join("else ", starts);
        var othersIf = string.Join("\n", others);
        var keywords = string.Join(",\n        ", _lexerBuilder.Lexemes
            .Where(lexeme => lexeme.Type == GenericToken.KeyWord)
            .Select(lexeme => $"{{ \"{lexeme.Name}\", {_lexerBuilder.LexerName}.{lexeme.Name} }}"));
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

    public IEnumerable<string> GenerateStart(Lexeme lexeme)
    {
        if (lexeme.Type == GenericToken.SugarToken)
        {
            List<string> sugarSteps = new List<string>();
            var pattern = lexeme.Args.First().Trim(new[] { '"' });
            if (!string.IsNullOrEmpty(pattern))
            {
                var newState = $"{States.InSugar}_{lexeme.Name}_1";
                _states.Add(newState);
                // start step
                var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.OtherStartTemplate), lexeme.Name,
                    additional: new Dictionary<string, string>()
                {
                    {"LEXEME_CONDITION", $"currentChar == '{pattern[0].ToString()}'" },
                    {"NEW_STATE", newState }
                });

                sugarSteps.Add(content);
                

                return sugarSteps;
            }
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
            _states.Add(nameof(States.InIdentifier));
            return new List<string>() { content };
        }
        if (lexeme.Type == GenericToken.Int)
        {

            var content = _templateEngine.ApplyTemplate(nameof(LexerTemplates.OtherStartTemplate), additional: new Dictionary<string, string>()
            {
                {"LEXEME_CONDITION", "char.IsDigit(currentChar)" },
                {"NEW_STATE", States.InInt.ToString() }
            });
            _longLexemes.Add(lexeme);
            _states.Add(nameof(States.InInt));
            return new List<string>() { content};
        }
        return new List<string>();
    }

    public List<string> GenerateOther(Lexeme lexeme)
    {
        if (lexeme.Type == GenericToken.SugarToken)
        {
            List<string> sugarSteps = new List<string>();
            string pattern = lexeme.Args.First().Trim(new[] { '"' });
            int last = 1;
            if (pattern.Length > 1)
            {

                for (int i = 1; i < pattern.Length; i++)
                {
                    last = i + 1;
                    var newState = $"{States.InSugar}_{lexeme.Name}_{i + 1}";
                    _states.Add(newState);
                    var stepContent = _templateEngine.ApplyTemplate(nameof(LexerTemplates.OtherStartTemplate), lexeme.Name,
                    additional: new Dictionary<string, string>()
                    {
                        {"LEXEME_CONDITION", $"state == LexerStates.{States.InSugar}_{lexeme.Name}_{i} && currentChar == '{pattern[i].ToString()}'" },
                        {"NEW_STATE", newState }
                    });
                    sugarSteps.Add("else "+stepContent);
                }
            }

            var lastStepContent = _templateEngine.ApplyTemplate(nameof(LexerTemplates.SugarTemplate), lexeme.Name,
                additional: new Dictionary<string, string>()
                {
                        {"LEXEME_CONDITION", $"state == LexerStates.{States.InSugar}_{lexeme.Name}_{last}" },
                        {"PATTERN", lexeme.Args.First().Trim(new[] { '"' }) },
                        {"NEW_STATE", "start" }
                });
            sugarSteps.Add(lastStepContent);
            return sugarSteps;
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
