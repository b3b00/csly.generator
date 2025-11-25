if (_currentState == <#STATE#>)
{
    var (ok, newState, match) = scanState_<#STATE#>(_currentPosition, source);
    if(ok)
    {
        if (match != null && match.IsDone)
        {
            tokens.Add(new Token<Toky>(match.Token, match.Value, match.Position));
            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
        }
        continue;
    }
    return "error !";
}