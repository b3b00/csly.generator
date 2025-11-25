public (bool ok, int newState, FsmMatch<<#LEXER#>> match) scanState_<#STATE_ID#>(LexerPosition position, ReadOnlySpan<char> source) {
    bool isEnd = <#IS_END#>;
    char ch = source[position.Index];

    <#TRANSITIONS#>
    
    if (isEnd) {
        _currentState = 0;
    // TODO : no more to consume store token in list and go back to start state
    <#ENDING#>
    _currentMatch.IsDone = true;
    return (true, 0, _currentMatch);
    }
    else {         
        return (false, -1, null);
    }   
}