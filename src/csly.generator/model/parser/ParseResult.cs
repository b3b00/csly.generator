using System.Diagnostics.CodeAnalysis;
using csly.generator.model.parser.tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csly.generator.model.parser;

public class ParseResult<IN, OUT> where IN : struct, Enum
{
    public OUT Result { get; set; }
    
    public ISyntaxNode<IN, OUT> SyntaxTree { get; set; }

    public bool IsError { get; set; }

    public bool IsOk => !IsError;

    public List<ParseError> Errors { get; set; }

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