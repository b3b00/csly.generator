if (_currentState == <#STATE#>)
{
    var(ok<#STATE#>, newState<#STATE#>, match<#STATE#>, isGoingEnd<#STATE#>) = scanState_<#STATE#>(_currentPosition, source);
    bool continueScanning = false;
    if(isGoingEnd<#STATE#> && match<#STATE#> != null)
    {
        _lastSuccessMatch = _currentMatch.Clone();
    }
    if (!ok<#STATE#> && _lastSuccessMatch != null)
    {
        //backtracking
        Func<FsmMatch <<#LEXER#>>, Token<<#LEXER#>>> factory;

        if (!_tokenFactories.TryGetValue(_lastSuccessMatch.Token, out factory))
        {
            factory = _defaultFactory;
        }
        var token = factory(_lastSuccessMatch);
        continueScanning = true;
        _currentState = 0;
        _currentPosition = ConsumeComments(token, source.ToArray());

        AddToken(token);
        _currentMatch = null;
        _lastSuccessMatch = null;
        //consume white spaces on token boundaries
        ConsumeWhiteSpace(source);
        // TODO : compute ending position = lastSuccessMatch.Position + lastSuccessMatch.Value.Length
        _currentPosition = token.Position.Forward(token.SpanValue.ToString());
        _startPosition = _currentPosition.Clone();
    }
    if (ok<#STATE#>)
    {
        continueScanning = true;
        if (match<#STATE#> != null && match<#STATE#>.IsDone)
        {
            
            // TODO : call action if any
            Func<FsmMatch <<#LEXER#>>, Token<<#LEXER#>>> factory;

            if (!_tokenFactories.TryGetValue(match<#STATE#>.Token, out factory))
            {
                factory = _defaultFactory;
            }
            var token = factory(match<#STATE#>);
            
            _currentPosition = ConsumeComments(token, source.ToArray());
            
            AddToken(token);
                _currentMatch = null;
            //consume whit spaces on token boundaries
            ConsumeWhiteSpace(source);

            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
        }
    }
    if (!continueScanning) {
        if (_currentPosition.Index >= source.Length)
        {
            _currentPosition.Index++; // to avoid infinite loop on end of source
            AddToken(new Token<<#LEXER#>>());
        }
        else
        {
            char ch = GetChar(source, _currentPosition);
            var error = new LexicalError(_currentPosition.Line, _currentPosition.Column, ch, "en");
            return error;
        }
    }
}