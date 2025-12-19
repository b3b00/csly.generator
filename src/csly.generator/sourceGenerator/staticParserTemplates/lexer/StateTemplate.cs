public (bool ok, int newState, FsmMatch<<#LEXER#>> match, bool isGoingEnd) scanState_<#STATE_ID#>(LexerPosition position, ReadOnlySpan<char> source) {
    bool isEnd = <#IS_END#>;
    char ch = GetChar(source, position);

    <#TRANSITIONS#>
    
    if (isEnd) {
        _currentState = 0;
    <#ENDING#>
    _currentMatch.IsDone = true;
    _lastSuccessMatch = _currentMatch.Clone();
    return (true, 0, _lastSuccessMatch, false);
    }
    else {         
        return (false, -1, null, false);
    }   
}