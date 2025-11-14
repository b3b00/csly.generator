
using System.Collections.Generic;

namespace <#NS#>;

internal class StaticLexerBuilder
{

    private readonly string _lexerName;

    public string LexerName => _lexerName;

    private List<Lexeme> _lexemes = new List<Lexeme>();

    public List<Lexeme> Lexemes => _lexemes;

    public StaticLexerBuilder(string lexerName)
    {
        _lexerName = lexerName;
    }


    public void AddLexeme(Lexeme lexeme)
    {
        _lexemes.Add(lexeme);
    }

    public void Add(GenericToken type, string name, params string[] args)
    {
        AddLexeme(new Lexeme(type, name, args));
    }
}
