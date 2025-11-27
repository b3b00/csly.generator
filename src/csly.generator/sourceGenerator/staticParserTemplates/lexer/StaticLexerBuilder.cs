using csly.generator.model.lexer;
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
}
