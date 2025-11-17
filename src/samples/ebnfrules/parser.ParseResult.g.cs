using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace csly.models;

public class ParseResult<IN, OUT> where IN : struct, Enum
{
    public OUT Result { get; set; }
    
    public ISyntaxNode<IN, OUT> SyntaxTree { get; set; }

    public bool IsError { get; set; }

    public bool IsOk => !IsError;

    public List<ParseError> Errors { get; set; }

    public ParseResult(OUT result, ISyntaxNode<IN, OUT> syntaxTree)
    {
        Result = result;
        SyntaxTree = syntaxTree;
        IsError = false;
        Errors = new List<ParseError>();
    }

    public ParseResult(List<ParseError> errors)
    {
        Errors = errors;
        IsError = true;
    }

    public ParseResult(string error)
    {        
        Errors = new List<ParseError>();
        Errors.Add(new LexicalError(0,0,'x',"TODO"));
        IsError = true;
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        if (IsOk)
        {
            return "parse OK.";
        }
        else
        {
            return $"parse failed : {string.Join("\n", Errors.Select(x => x.ErrorMessage))}";
        }
    }
}