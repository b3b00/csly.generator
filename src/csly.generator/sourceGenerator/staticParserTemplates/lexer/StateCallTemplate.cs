if (_currentState == <#STATE#>)
{
    var(ok<#STATE#>, newState<#STATE#>, match<#STATE#>) = scanState_<#STATE#>(_currentPosition, source);
    bool continueScanning = false;
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
            AddToken(token);
                _currentMatch = null;
            //consume whit spaces on token boundaries
            ConsumeWhitSpace(source);

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
            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
        }
    }
}