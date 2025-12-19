using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System;

namespace <#NS#>;


public sealed class LexerPosition : IComparable
{

    public LexerPosition() : this(0, 0, 0)
    {
    }

    public LexerPosition(int index, int line, int column, string mode = ModeAttribute.DefaultLexerMode)
    {
        Index = index;
        Line = line;
        Column = column;
        Mode = mode;
        Indentation = null;
    }

    private LexerPosition(int index, int line, int column, int currentIndentation,
        string mode = ModeAttribute.DefaultLexerMode) : this(index, line, column, mode)
    {
        CurrentIndentation = currentIndentation;
        Indentation = new LexerIndentation();
    }

    public bool IsStartOfLine => Column == 0;

    public int CurrentIndentation { get; set; }

    [JsonIgnore] public LexerIndentation Indentation { get; set; }

    public int Column { get; set; }

    public int Index { get; set; }

    public int Line { get; set; }

    public string Mode { get; set; }

    public bool IsPop { get; set; }

    public bool IsPush { get; set; }

    public override string ToString()
    {
        return $"line {Line}, column {Column}";
    }

    [ExcludeFromCodeCoverage]
    public int CompareTo(object obj)
    {
        if (obj is not LexerPosition position) return 1;

        if (Line < position.Line)
        {
            return -1;
        }

        if (Line == position.Line)
        {
            return Column.CompareTo(position.Column);
        }

        return 1;
    }

    public LexerPosition Back(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Clone();
        }

        int charCount = text.Length;
        int newLine = Line;
        int newColumn = Column;

        // Process the text backwards to count newlines and compute new column
        for (int i = text.Length - 1; i >= 0; i--)
        {
            char c = text[i];

            if (c == '\n')
            {
                newLine--;
                // For LF or CRLF, we need to find the column position on the previous line
                // We can't know the exact column without the full source, so set to 0
                newColumn = 0;
            }
            else if (c == '\r' && (i + 1 >= text.Length || text[i + 1] != '\n'))
            {
                // Standalone CR (not part of CRLF)
                newLine--;
                newColumn = 0;
            }
            else if (c != '\r') // Skip \r if it's part of CRLF
            {
                if (newColumn > 0 || newLine == Line)
                {
                    newColumn--;
                }
            }
        }

        return new LexerPosition(Index - charCount, newLine, newColumn, CurrentIndentation, Mode)
        {
            Indentation = this.Indentation?.Clone(),
            IsPop = IsPop,
            IsPush = IsPush,
        };
    }

    public LexerPosition Forward(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Clone();
        }

        int charCount = text.Length;
        int newLine = Line;
        int newColumn = Column;

        // Process the text forwards to count newlines and compute new column
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '\n')
            {
                newLine++;
                newColumn = 0;
            }
            else if (c == '\r')
            {
                // Check if this is CRLF or standalone CR
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    // CRLF - skip the \r, the \n will be processed next
                    continue;
                }
                else
                {
                    // Standalone CR
                    newLine++;
                    newColumn = 0;
                }
            }
            else
            {
                newColumn++;
            }
        }

        return new LexerPosition(Index + charCount, newLine, newColumn, CurrentIndentation, Mode)
        {
            Indentation = this.Indentation?.Clone(),
            IsPop = IsPop,
            IsPush = IsPush,
        };
    }


    public LexerPosition Clone()
    {
        return new LexerPosition(Index, Line, Column, CurrentIndentation)
        {
            Indentation = this.Indentation?.Clone(),
            Mode = Mode,
            IsPop = IsPop,
            IsPush = IsPush,
        };
    }

    public static bool operator ==(LexerPosition p1, LexerPosition p2)
    {
        return p1.Index == p2.Index;
    }

    public static bool operator !=(LexerPosition p1, LexerPosition p2)
    {
        return p1.Index != p2.Index;
    }

    public bool Equals(LexerPosition p)
    {
        return Index == p.Index;
    }

    public override bool Equals(object obj)
    {
        if (obj is LexerPosition position)
        {
            return Equals(position);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }
}