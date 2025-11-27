
using ebnf.grammar;
using System;
using csly.ebnf.models;
using System.Collections.Generic;
using ebnf;
using Factory = System.Func<csly.ebnf.models.FsmMatch<ebnf.EbnfTokenGeneric>, csly.ebnf.models.Token<ebnf.EbnfTokenGeneric>>;

namespace ebnf.grammar
{


    public enum LexerStates
    {
        Start,
        InIdentifier, InSugar_COLON_1, InSugar_ZEROORMORE_1, InSugar_ONEORMORE_1, InSugar_OPTION_1, InSugar_DISCARD_1, InSugar_LPAREN_1, InSugar_RPAREN_1, InSugar_OR_1, InSugar_LCROG_1, InSugar_RCROG_1, InSugar_DASH_1, InSugar_LCURLY_1, InSugar_RCURLY_1, InInt, InSugar_DISCARD_2, InSugar_DISCARD_3
    }

    public class StaticEbnfTokenGeneric
    {

        private int _currentState = 0;

        private FsmMatch<EbnfTokenGeneric> _currentMatch = null;

        private LexerPosition _currentPosition { get; set; }

        private LexerPosition _startPosition { get; set; }

        private char GetChar(ReadOnlySpan<char> source, LexerPosition position)
        {
            if (position.Index >= source.Length)
            {
                return '\0';
            }
            return source[position.Index];
        }

        private Dictionary<string, EbnfTokenGeneric> _keywords = new Dictionary<string, EbnfTokenGeneric>()
        {

        };

        private Dictionary<EbnfTokenGeneric, Factory> _tokenFactories = new Dictionary<EbnfTokenGeneric, Factory>();

        private Factory _defaultFactory;

        public StaticEbnfTokenGeneric()
        {
            _defaultFactory = match => new Token<EbnfTokenGeneric>(match.Token, match.Value, match.Position);
            _tokenFactories.Add(EbnfTokenGeneric.IDENTIFIER, match =>
            {
                if (_keywords.TryGetValue(match.Value.ToString(), out var keywordToken))
                {
                    return new Token<EbnfTokenGeneric>(keywordToken, match.Value, match.Position);
                }
                return new Token<EbnfTokenGeneric>(match.Token, match.Value, match.Position);
            });
        }



        /// consumes all whitspaces starting from _currentPosition and move _currentPosition accordingly
        private void ConsumeWhitSpace(ReadOnlySpan<char> source)
        {
            while (_currentPosition.Index < source.Length && char.IsWhiteSpace(source[_currentPosition.Index]))
            {
                if (source[_currentPosition.Index] == '\n')
                {
                    _currentPosition.Line++;
                    _currentPosition.Column = 0;
                }
                else
                {
                    _currentPosition.Column++;
                }
                _currentPosition.Index++;
            }
        }

        public LexerResult<EbnfTokenGeneric> Scan(ReadOnlySpan<char> source)
        {
            _currentPosition = new LexerPosition(0, 0, 0);
            _startPosition = new LexerPosition(0, 0, 0);
            List<Token<EbnfTokenGeneric>> tokens = new List<Token<EbnfTokenGeneric>>();


            void AddToken(Token<EbnfTokenGeneric> token)
            {
                tokens.Add(token);
            }

            while (_currentPosition.Index <= source.Length)
            {



                if (_currentState == 0)
                {
                    var (ok0, newState0, match0) = scanState_0(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok0)
                    {
                        continueScanning = true;
                        if (match0 != null && match0.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match0.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match0);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 1)
                {
                    var (ok1, newState1, match1) = scanState_1(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok1)
                    {
                        continueScanning = true;
                        if (match1 != null && match1.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match1.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match1);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 2)
                {
                    var (ok2, newState2, match2) = scanState_2(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok2)
                    {
                        continueScanning = true;
                        if (match2 != null && match2.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match2.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match2);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 3)
                {
                    var (ok3, newState3, match3) = scanState_3(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok3)
                    {
                        continueScanning = true;
                        if (match3 != null && match3.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match3.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match3);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 4)
                {
                    var (ok4, newState4, match4) = scanState_4(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok4)
                    {
                        continueScanning = true;
                        if (match4 != null && match4.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match4.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match4);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 5)
                {
                    var (ok5, newState5, match5) = scanState_5(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok5)
                    {
                        continueScanning = true;
                        if (match5 != null && match5.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match5.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match5);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 6)
                {
                    var (ok6, newState6, match6) = scanState_6(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok6)
                    {
                        continueScanning = true;
                        if (match6 != null && match6.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match6.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match6);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 7)
                {
                    var (ok7, newState7, match7) = scanState_7(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok7)
                    {
                        continueScanning = true;
                        if (match7 != null && match7.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match7.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match7);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 8)
                {
                    var (ok8, newState8, match8) = scanState_8(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok8)
                    {
                        continueScanning = true;
                        if (match8 != null && match8.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match8.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match8);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 9)
                {
                    var (ok9, newState9, match9) = scanState_9(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok9)
                    {
                        continueScanning = true;
                        if (match9 != null && match9.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match9.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match9);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 10)
                {
                    var (ok10, newState10, match10) = scanState_10(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok10)
                    {
                        continueScanning = true;
                        if (match10 != null && match10.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match10.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match10);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 11)
                {
                    var (ok11, newState11, match11) = scanState_11(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok11)
                    {
                        continueScanning = true;
                        if (match11 != null && match11.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match11.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match11);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 12)
                {
                    var (ok12, newState12, match12) = scanState_12(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok12)
                    {
                        continueScanning = true;
                        if (match12 != null && match12.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match12.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match12);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 13)
                {
                    var (ok13, newState13, match13) = scanState_13(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok13)
                    {
                        continueScanning = true;
                        if (match13 != null && match13.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match13.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match13);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 14)
                {
                    var (ok14, newState14, match14) = scanState_14(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok14)
                    {
                        continueScanning = true;
                        if (match14 != null && match14.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match14.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match14);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 15)
                {
                    var (ok15, newState15, match15) = scanState_15(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok15)
                    {
                        continueScanning = true;
                        if (match15 != null && match15.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match15.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match15);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 16)
                {
                    var (ok16, newState16, match16) = scanState_16(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok16)
                    {
                        continueScanning = true;
                        if (match16 != null && match16.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match16.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match16);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 17)
                {
                    var (ok17, newState17, match17) = scanState_17(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok17)
                    {
                        continueScanning = true;
                        if (match17 != null && match17.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match17.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match17);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
                else if (_currentState == 18)
                {
                    var (ok18, newState18, match18) = scanState_18(_currentPosition, source);
                    bool continueScanning = false;
                    if (ok18)
                    {
                        continueScanning = true;
                        if (match18 != null && match18.IsDone)
                        {

                            // TODO : call action if any
                            Func<FsmMatch<EbnfTokenGeneric>, Token<EbnfTokenGeneric>> factory;

                            if (!_tokenFactories.TryGetValue(match18.Token, out factory))
                            {
                                factory = _defaultFactory;
                            }
                            var token = factory(match18);
                            AddToken(token);
                            _currentMatch = null;
                            //consume whit spaces on token boundaries
                            ConsumeWhitSpace(source);

                            _startPosition = new LexerPosition(_currentPosition.Index, _currentPosition.Line, _currentPosition.Column);
                        }
                    }
                    if (!continueScanning)
                    {
                        if (_currentPosition.Index >= source.Length)
                        {
                            _currentPosition.Index++; // to avoid infinite loop on end of source
                            AddToken(new Token<EbnfTokenGeneric>());
                        }
                        else
                        {
                            return $"error @ {_currentPosition.ToString()} on character '{source[_currentPosition.Index]}'";
                        }
                    }
                }
            }
            return tokens;
        }


        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_0(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = false;
            char ch = GetChar(source, position);


            if (ch == '-')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '_')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch >= 'a' && ch <= 'z')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch >= 'A' && ch <= 'Z')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch >= '0' && ch <= '9')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == ':')
            {

                _currentState = 2;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '*')
            {

                _currentState = 3;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '+')
            {

                _currentState = 4;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '?')
            {

                _currentState = 5;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '[')
            {

                _currentState = 6;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '(')
            {

                _currentState = 9;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == ')')
            {

                _currentState = 10;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '|')
            {

                _currentState = 11;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == ']')
            {

                _currentState = 12;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '\'')
            {

                _currentState = 13;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '{')
            {

                _currentState = 16;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '}')
            {

                _currentState = 17;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch >= '0' && ch <= '9')
            {

                _currentState = 18;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state

                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_1(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);


            if (ch == '-')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER, memory, _startPosition);
                return (true, _currentState, _currentMatch);

            }

            if (ch == '_')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER, memory, _startPosition);
                return (true, _currentState, _currentMatch);

            }

            if (ch >= 'a' && ch <= 'z')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER, memory, _startPosition);
                return (true, _currentState, _currentMatch);

            }

            if (ch >= 'A' && ch <= 'Z')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER, memory, _startPosition);
                return (true, _currentState, _currentMatch);

            }

            if (ch >= '0' && ch <= '9')
            {

                _currentState = 1;
                _currentPosition.Index++;
                _currentPosition.Column++;
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER, memory, _startPosition);
                return (true, _currentState, _currentMatch);

            }

            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_2(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.COLON, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_3(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.ZEROORMORE, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_4(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.ONEORMORE, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_5(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.OPTION, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_6(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);


            if (ch == 'd')
            {

                _currentState = 7;
                _currentPosition.Index++;
                _currentPosition.Column++;
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.DISCARD, memory, _startPosition);
                return (true, _currentState, _currentMatch);

            }

            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_7(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = false;
            char ch = GetChar(source, position);


            if (ch == ']')
            {

                _currentState = 8;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state

                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_8(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.DISCARD, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_9(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_10(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.RPAREN, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_11(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.OR, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_12(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.RCROG, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_13(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = false;
            char ch = GetChar(source, position);


            if (ch != '\'' && ch != '\\')
            {

                _currentState = 13;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '\\')
            {

                _currentState = 14;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (ch == '\'')
            {

                _currentState = 15;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state

                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_14(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = false;
            char ch = GetChar(source, position);


            if (true)
            {

                _currentState = 13;
                _currentPosition.Index++;
                _currentPosition.Column++;

                return (true, _currentState, _currentMatch);

            }

            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state

                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_15(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.STRING, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_16(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.LCURLY, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_17(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);



            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.RCURLY, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
        public (bool ok, int newState, FsmMatch<EbnfTokenGeneric> match) scanState_18(LexerPosition position, ReadOnlySpan<char> source)
        {
            bool isEnd = true;
            char ch = GetChar(source, position);


            if (ch >= '0' && ch <= '9')
            {

                _currentState = 18;
                _currentPosition.Index++;
                _currentPosition.Column++;
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.INT, memory, _startPosition);
                return (true, _currentState, _currentMatch);

            }

            if (isEnd)
            {
                _currentState = 0;
                // TODO : no more to consume store token in list and go back to start state
                var sliced = source.Slice(_startPosition.Index, _currentPosition.Index - _startPosition.Index);
                var memory = new ReadOnlyMemory<char>(sliced.ToArray());
                _currentMatch = new FsmMatch<EbnfTokenGeneric>(EbnfTokenGeneric.INT, memory, _startPosition);
                _currentMatch.IsDone = true;
                return (true, 0, _currentMatch);
            }
            else
            {
                return (false, -1, null);
            }
        }
    }
}