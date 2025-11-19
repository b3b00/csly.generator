

namespace <#NS#>;

public class LexicalError : ParseError
{
    public LexicalError(int line, int column, char unexpectedChar, string i18n)
    {
        Line = line;
        Column = column;
        UnexpectedChar = unexpectedChar;
        ErrorType = ErrorType.UnexpectedChar;
        _i18N = i18n;
    }

    protected readonly string _i18N;

    public char UnexpectedChar { get; set; }

    public override string ErrorMessage => $"Unexpected char '{UnexpectedChar}' ({(int)UnexpectedChar} at {Line},{Column}";

    public override string ToString()
    {
        return ErrorMessage;
    }

    protected override string GetContextualMessage(string fullSource)
    {
        /*var message = I18N.Instance.GetText(_i18N, I18NMessage.UnexpectedCharacter,
            UnexpectedChar.ToString());
        return GetContextualMessage(fullSource, Line, Column, message);*/
        return "TODO";
    }
}