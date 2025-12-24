using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csly.generator.model.lexer;

namespace csly.generator.sourceGenerator.fsm;

internal class State
{
    public string Name { get; set; }

    public int Id { get; set; }
    

    public bool IsEnd { get; set; }

    public bool IsExplicitEnd { get; set; }

    private string _tokenName;

    public bool IsSingleLineComment { get; set; } = false;

    public bool IsMultiLineComment { get; set; } = false;

    public string MultiLineCommentEndString { get; set; }

    public bool IsPop { get; set; } = false;

    public string PushTarget { get; set; } = null;


    public string TokenName
    {
        get => _tokenName; set
        {
            if (!string.IsNullOrEmpty(_tokenName) && value == null)
            {
                ;
            }
            _tokenName = value;
        }
    }
    public State(int id, bool isEnd = false, bool isPop = false, string pushTarget = null)
    {
        Id = id;
        IsEnd = isEnd;
        IsPop = isPop;
        PushTarget = pushTarget;
    }
}

internal class Transition
{
    
    public int TargetState { get; set; }

    public Func<char, bool> Condition { get; set; }

    public string StringCondition { get; set; }

    public bool IsException { get; set; }

    public Transition(int targetState, Func<char, bool> condition, string stringCondition)
    {
        TargetState = targetState;
        Condition = condition;
        StringCondition = stringCondition;
    }



}





internal class Fsm
{

    public List<Lexeme> Lexemes { get; set; } = new List<Lexeme>();

    private Dictionary<int, State> _states = new Dictionary<int, State>();

    private Dictionary<string, State> _statesByName = new Dictionary<string, State>();

    private Dictionary<int, List<Transition>> _transitions = new Dictionary<int, List<Transition>>();

    public List<State> States => _states.Values.ToList();

    public Dictionary<string, string> Keywords => _keywords;

    public List<string> ExplicitKeywords => _explicitKeywords;

    private Dictionary<string, string> _factories = new Dictionary<string, string>();
    public Dictionary<string, string> Factories => _factories;


    int _currentState = 0;
    private Dictionary<string, string> _keywords = new Dictionary<string, string>();

    private List<string> _explicitKeywords = new List<string>();

    

    public Fsm()
    {
        State startState = new State(0);
        startState.Name = "start";
        AddState(startState);
        _currentState = startState.Id;

    }

    public List<int> GetEpsilonStates()
    {
        return _states.Values.Where(s => _transitions.ContainsKey(s.Id) == false).Select(s => s.Id).ToList();
    }

    public bool IsEpsilonState(int stateId)
    {
        return _transitions.ContainsKey(stateId) == false;
    }

    private string EscapeChar(char ch)
    {
        switch (ch)
        {
            case '\n':
                return "\\n";
            case '\r':
                return "\\r";
            case '\t':
                return "\\t";
            case '\\':
                return "\\\\";       
            case '\'':
                return "\\'";
        }
        return ch.ToString();
            }

    public void AddState(State state)
    {
        _states[state.Id] = state;
        if (!string.IsNullOrEmpty(state.Name))
        {
            _statesByName[state.Name] = state;
        }
    }

    public void Pop()
    {
        var state = GetState(_currentState);
        if (state != null)
        {
            state.IsPop = true;
        }
    }

    public void PushTo(string target)
    {
        var state = GetState(_currentState);
        if (state != null)
        {
            state.PushTarget = target;
        }
    }

    public void GoTo(int state)
    {
        _currentState = state;
    }

    public void GoTo(string stateName)
    {
        if (_statesByName.TryGetValue(stateName, out var state))
        {
            _currentState = state.Id;
        }
        else
        {
            throw new Exception($"State '{stateName}' not found.");
        }
    }

    private int GetNewState()
    {
        int id = _states.Count;        
        return id;
    }

public void End(Lexeme lexeme, bool isSingleLineComment = false, bool isMultiLineComment = false)
    {
        End(lexeme.Name, lexeme.IsExplicit, isSingleLineComment, isMultiLineComment);
        if (lexeme.IsPop)
        {
            Pop();
        }
        if (lexeme.IsPush)
        {
            PushTo(lexeme.PushTarget);
        }
    }

    public void End(string tokenName, bool isExplicit = false, bool isSingleLineComment = false, bool isMultiLineComment = false)
    {
        if (_states.TryGetValue(_currentState, out var state))
        {
            state.IsEnd = true;
            state.TokenName = tokenName;
            state.IsExplicitEnd = isExplicit;
            state.IsSingleLineComment = isSingleLineComment;
            state.IsMultiLineComment = isMultiLineComment;
        }
    }

    public void Mark(string name)
    {
        if (_states.TryGetValue(_currentState, out var state))
        {
            state.Name = name;
            _statesByName[name] = state;
        }
    }

    #region transitions

    public void Transition(char input)
    {
        int id = GetNewState();
        State state = new State(id);
        AddState(state);
        TransitionTo(state.Id, input);
    }

    public void AnyTransition()
    {
        int id = GetNewState();
        State state = new State(id);
        AddState(state);
        AnyTransitionTo(state.Id);
    }

    public void AnyTransitionTo(int targetStateId)
    {
        Transition transition = new Transition(targetStateId, (ch) => true, "true");
        if (!_transitions.ContainsKey(_currentState))
        {
            _transitions[_currentState] = new List<Transition>();
        }
        _transitions[_currentState].Add(transition);
        _currentState = targetStateId;
    }

    public void AnyTransitionTo(string targetState)
    {
        if (_statesByName.TryGetValue(targetState, out var state))
        {
            AnyTransitionTo(state.Id);
        }
        else
        {
            throw new Exception($"State '{targetState}' not found.");
        }
    }

    public Transition GetTransition(char input)
    {
        if (_transitions.TryGetValue(_currentState, out var transitions))
        {
            foreach (var transition in transitions)
            {
                if (transition.Condition(input))
                {
                    return transition;
                }
            }
        }
        return null;
    }

    public void TransitionTo(string targetStateName, char input)
    {
        if (_statesByName.TryGetValue(targetStateName, out var state))
        {
            TransitionTo(state.Id, input);
        }
        else
        {
            throw new Exception($"State '{targetStateName}' not found.");
        }
    }

    public void TransitionTo(int newStateId, char input)
    {
        Transition transition = new Transition(newStateId, (ch) => ch == input, $"ch == '{EscapeChar(input)}'");
        if (!_transitions.ContainsKey(_currentState))
        {
            _transitions[_currentState] = new List<Transition>();
        }
        _transitions[_currentState].Add(transition);
        _currentState = newStateId;
    }

    public void SafeTransition(char input)
    {
        var transition = GetTransition(input);
        if (transition != null)
        {
            _currentState = transition.TargetState;
        }
        else
        {
            Transition(input);
        }
    }

    public int RangeTransition(char start, char end)
    {
        int id = GetNewState();
        State state = new State(id);
        _states[id] = state;
        RangeTransitionTo(state.Id, start, end);
        return id;
    }

    public int RangeTransitionTo(string targetStateName, char start, char end)
    {
        if (_statesByName.TryGetValue(targetStateName, out var state))
        {
            return RangeTransitionTo(state.Id, start, end);
        }
        else
        {
            throw new Exception($"State '{targetStateName}' not found.");
        }
    }

    public int RangeTransitionTo(int target, char start, char end)
    {
        Transition transition = new Transition(target, (ch) => ch >= start && ch <= end, $"ch >= '{EscapeChar(start)}' && ch <= '{EscapeChar(end)}'");
        if (!_transitions.ContainsKey(_currentState))
        {
            _transitions[_currentState] = new List<Transition>();
        }
        _transitions[_currentState].Add(transition);
        _currentState = target;
        return target;
    }

    public int MultiRangeTransition(params (char start, char end)[] ranges)
    {
        int id = GetNewState();
        State state = new State(id);
        AddState(state);
        return MultiRangeTransitionTo(state.Id, ranges);

    }

    public int MultiRangeTransitionTo(string targetStateName, params (char start, char end)[] ranges)
    {
        if (_statesByName.TryGetValue(targetStateName, out var state))
        {
            return MultiRangeTransitionTo(state.Id, ranges);
        }
        else
        {
            throw new Exception($"State '{targetStateName}' not found.");
        }
    }

    public int MultiRangeTransitionTo(int target, params (char start, char end)[] ranges)
    {
        var cond = String.Join(" || ", ranges.Select(x => $"ch >= '{EscapeChar(x.start)}' && ch <= '{EscapeChar(x.end)}'"));

        Transition transition = new Transition(target, (ch) =>
        {
            foreach (var range in ranges)
            {
                if (ch >= range.start && ch <= range.end)
                {
                    return true;
                }
            }
            return false;
        }, cond);
        if (!_transitions.ContainsKey(_currentState))
        {
            _transitions[_currentState] = new List<Transition>();
        }
        _transitions[_currentState].Add(transition);
        _currentState = target;
        return target;
    }

    public int ExceptTransition(string except)
    {
        int id = GetNewState();
        State state = new State(id);
        AddState(state);
        return ExceptTransitionTo(state.Id, except);
    }

    public int ExceptTransitionTo(string targetStateName, string except)
    {
        if (_statesByName.TryGetValue(targetStateName, out var state))
        {
            return ExceptTransitionTo(state.Id, except);
        }
        else
        {
            throw new Exception($"State '{targetStateName}' not found.");
        }
    }

    public int ExceptTransitionTo(int target, string except)
    {
        if (_transitions.TryGetValue(target, out var transitions)) {
            var exception = transitions.FirstOrDefault(x => x.IsException);
            if (exception != null)
            {
                var exceptChars = except.ToCharArray();
                string exceptCondition = string.Join(" && ", exceptChars.Select(x => $"ch != '{EscapeChar(x)}'"));
                exception.StringCondition += " && " + exceptCondition;
                exception.Condition = (ch) => exception.Condition(ch) && exceptChars.All(x => x != ch);                
                return exception.TargetState;
            }            
        }
        if (_states.ContainsKey(target) == false)
        {
            var newStateId = GetNewState();
            State state = new State(newStateId);
            AddState(state);
            //_states[newStateId] = state;
            target = newStateId;
        }
        var chars = except.ToCharArray();
        string cond = string.Join(" && ", chars.Select(x => $"ch != '{EscapeChar(x)}'"));

        Transition transition = new Transition(target, (ch) => chars.All(x => x != ch), cond)
        {
            IsException = true
        };
        if (!_transitions.ContainsKey(_currentState))
        {
            _transitions[_currentState] = new List<Transition>();
        }
        _transitions[_currentState].Add(transition);
        _currentState = target;
        return target;
    }

    public int ConstantTransition(string constant)
    {
        var c = constant[0];
        for (var i = 0; i < constant.Length; i++)
        {
            c = constant[i];
            SafeTransition(c);
        }
        return _currentState;
    }

    internal List<Transition> GetTransitions(int id)
    {
        if (_transitions.TryGetValue(id, out var transitions))
        {
            return transitions;
        }
        return new List<Transition>();

    }

    internal State GetState(int targetState)
    {
        return _states[targetState];
    }

    internal State GetCurrentState()
    {
        return _states[_currentState];
    }

    internal State GetState(string stateName)
    {
        if (_statesByName.TryGetValue(stateName, out var state))
        {
            return state;
        }
        return null;
    }

    internal void AddKeyword(string arg0, string name)
    {
        _keywords[arg0] = name;
    }

    internal void AddExplicitKeyword(string pattern)
    {
        _explicitKeywords.Add(pattern);
    }

    internal void AddFactory(string tokenName, string factory)
    {
        if (!_factories.ContainsKey(tokenName))
        {
            _factories[tokenName] = factory;
        }
    }

    #endregion


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var state in _states.Values)
        {
            var name = !string.IsNullOrEmpty(state.Name) ? $"[{state.Name}]" : "";
            sb.AppendLine($"State {state.Id}{name} {(state.IsEnd ? "(End)" : "")} {(string.IsNullOrEmpty(state.Name) ? "" : $"Name: {state.Name}")} Token: {state.TokenName} --  {(state.IsMultiLineComment ? "multi line comment" : "")} {(state.IsSingleLineComment ? "single line comment" : "")}");
            foreach (var transition in GetTransitions(state.Id))
            {
                var target = GetState(transition.TargetState);
                var targetName = !string.IsNullOrEmpty(target.Name) ? $"[{target.Name}]" : "";
                sb.AppendLine($"\t-- [{transition.StringCondition}] => State {target.Id}{targetName} {(target.IsEnd ? "(END)": "")}");
            }

        }
        return sb.ToString();
    }
}
