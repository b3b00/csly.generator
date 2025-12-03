using csly.generator.model.lexer;
using ebnf.grammar;
using System;
using System.Collections.Generic;
using System.Linq;

internal class StaticLexerBuilder
{

    private readonly string _lexerName;

    public string LexerName => _lexerName;

    private List<Lexeme> _lexemes = new List<Lexeme>();

    public List<Lexeme> Lexemes => _lexemes;


    private readonly string _nameSpace;
    public string NameSpace => _nameSpace;

    

    public StaticLexerBuilder(string lexerName, string nameSpace)
    {
        _lexerName = lexerName;
        _nameSpace = nameSpace;
    }


    public void AddLexeme(Lexeme lexeme)
    {
        _lexemes.Add(lexeme);
    }

    public void Add(GenericToken type, string name, List<string> modes, params string[] args)
    {
        var lexem = new Lexeme(type, name, args);
        lexem.Modes = modes != null && modes.Any() ? modes : new List<string>() { "default"};        
        AddLexeme(lexem);
    }


    internal void SetExplicitTokens(List<TerminalClause> explicitTokens)
    {
        var id = _lexemes.FirstOrDefault(x => x.Type == GenericToken.Identifier);

        foreach (var token in explicitTokens)
        {            
            token.Name = token.Name.Trim('\'');
            if (MatchId(id, token.Name))
            {                
                AddLexeme(new Lexeme(GenericToken.KeyWord, token.Name));
            }
            else
            {
                AddLexeme(new Lexeme(GenericToken.Identifier, token.Name));                
            }
        }
    }

    private bool MatchId(Lexeme id,string token)
    {
        if (string.IsNullOrEmpty(token) || id == null)
        {
            return false;
        }
        bool match = Match(id.IdentifierStartPatterns(), token[0]);
        int i = 1;
        while (match && i < token.Length)
        {            
            match &= Match(id.IdentifierTailPatterns(), token[i]);
            i++;
        }
        return match;
    }

    private bool Match(IEnumerable<char[]> pattern, char c)
    {
        
        bool match = pattern.Any(pattern =>
        {
            if (pattern.Length == 1)
            {
                return pattern[0] == c;
            }
            else if (pattern.Length == 2)
            {
                return c >= pattern[0] && c <= pattern[1];
            }
            return false;
        });
        return match;
    }
}
