using System;
using System.Collections.Generic;
using System.Text;
using csly.<#ASSEMBLY#>.<#PARSER_NS#>.models;

namespace <#NAMESPACE#>.<#PARSER_NS#>;

    public interface ISubLexer
    {

        string Name { get; }

    (LexerResult<<#LEXER#>> Result, LexerPosition NewPosition, bool isPop, string PushTarget) Scan(ReadOnlySpan<char> source, LexerPosition position);




    }

