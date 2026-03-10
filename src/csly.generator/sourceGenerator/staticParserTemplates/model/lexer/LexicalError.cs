
namespace <#NS#>;

public class LexicalError : ParseError
{
    public LexicalError(LexerPosition position, char unexpectedChar, string i18n)
    {
        Position = position;
        UnexpectedChar = unexpectedChar;
        ErrorType = ErrorType.UnexpectedChar;
        _i18N = i18n;
    }

    public LexicalError(string message)
    {
        ErrorType = ErrorType.UnexpectedChar;
    }

    public string Message { get; set; }

    protected readonly string _i18N;

    public char UnexpectedChar { get; set; }

    public override string ErrorMessage => GetErrorMessage();

    private string GetErrorMessage()
    {
        if (UnexpectedChar != (char)10 && UnexpectedChar != (char)13)
        {
            return string.IsNullOrEmpty(Message) ? $"Unexpected char '{UnexpectedChar}' ({(int)UnexpectedChar}) at {Line},{Column}" : Message;
        }
        else
        {
            var charName = UnexpectedChar == (char)10 ? "LF" : "CR";
            return string.IsNullOrEmpty(Message) ? $"Unexpected char '{charName}' ({(int)UnexpectedChar}) at {Line},{Column}" : Message;
        }
    }
    
    public override int Line => Position.Line;
    public override int Column => Position.Column;

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