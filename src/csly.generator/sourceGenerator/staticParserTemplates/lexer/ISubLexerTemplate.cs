using System;
using System.Collections.Generic;
using System.Text;
using csly.<#ASSEMBLY#>.models;

namespace <#NAMESPACE#>;

    public interface ISubLexer
    {

        string Name { get; }

    (LexerResult<WhileTokenGeneric> Result, LexerPosition NewPosition, bool isPop, string PushTarget) Scan(ReadOnlySpan<char> source, LexerPosition position);




    }

