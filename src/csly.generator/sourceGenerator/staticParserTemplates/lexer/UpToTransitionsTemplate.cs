    else
    {
            ReadOnlyMemory<char> shorterValue = source.Slice(_currentPosition.Index, source.Length - _currentPosition.Index).ToArray().AsMemory();            
            string shorterDelimiter = "";
            bool found = false;
            <#LEXER#> shorterToken = default(<#LEXER#>);
            foreach (var upto in _uptoTOkens)
            {
                var m = new ReadOnlyMemory<char>(source.ToArray());
                var up = m.GetUpTo(upto.Key, _currentPosition.Index);
                if (up.Length > 0)
                {   
                    found = true;
                    if ( up.Length < shorterValue.Length)
                    {
                        shorterDelimiter = upto.Key;
                        shorterValue = up;
                        shorterToken = upto.Value;
                    }
                }
            }

            if (found)
                {
                    // generate token
                    var memory = new ReadOnlyMemory<char>(source.Slice(_currentPosition.Index, shorterValue.Length).ToArray());
                    var v = memory.ToString();
                    var token = new Token<<#LEXER#>>(shorterToken, memory, _currentPosition);
                    _currentPosition = _currentPosition.Forward(shorterValue.ToString());
                    _startPosition = _currentPosition.Clone();
                    // TODO : set _currentMatch and _lastSuccessMatch
                    _currentMatch = new FsmMatch<<#LEXER#>>(shorterToken, memory, position)
                    {
                        IsPop = false,
                        PushTarget = "",
                        IsDone = true
                    };
                    return (true, _currentState, _currentMatch, true);
                }
    }