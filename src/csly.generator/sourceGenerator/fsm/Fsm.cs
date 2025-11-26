using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csly.generator.sourceGenerator.fsm;

internal class State
{
    public string Name { get; set; }

    public int Id { get; set; }
    

    public bool IsEnd { get; set; }

    public string TokenName { get; set; }
    public State(int id, bool isEnd = false)
    {
        Id = id;
        IsEnd = isEnd;
    }
}

internal class Transition
{
    
    public int TargetState { get; set; }

    public Func<char, bool> Condition { get; set; }

    public string StringCondition { get; set; }

    public Transition(int targetState, Func<char, bool> condition, string stringCondition)
    {
        TargetState = targetState;
        Condition = condition;
        StringCondition = stringCondition;
    }

}





internal class Fsm
{      

    private Dictionary<int, State> _states = new Dictionary<int, State>();

    private Dictionary<string, State> _statesByName = new Dictionary<string, State>();

    private Dictionary<int, List<Transition>> _transitions = new Dictionary<int, List<Transition>>();

    public List<State> States => _states.Values.ToList();

    public Dictionary<string, string> Keywords => _keywords;


    private Dictionary<string, string> _factories = new Dictionary<string, string>();
    public Dictionary<string, string> Factories => _factories;


    int _currentState = 0;
    private Dictionary<string,string> _keywords = new Dictionary<string, string>();

    public Fsm()
    {
        State startState = new State(0);
        startState.Name = "start";
        AddState(startState);
        _currentState = startState.Id;
        
    }

    public void AddState(State state)
    {
        _states[state.Id] = state;
        if (!string.IsNullOrEmpty(state.Name)) 
        {
            _statesByName[state.Name] = state;
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

    private int GetNewState()     {
        return _states.Count;
    }

    public void End(string tokenName)
    {
        if (_states.TryGetValue(_currentState, out var state))
        {
            state.IsEnd = true;
            state.TokenName = tokenName;
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
        _states[id] = state;        
        TransitionTo(state.Id, input);        
    }

    private Transition GetTransition(char input)
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

    public void TransitionTo(int target, char input)
    {
        var newStateId = GetNewState();
        State state = new State(newStateId);
        _states[newStateId] = state;
        Transition transition = new Transition(newStateId, (ch) => ch == input, $"ch == '{input}'");
        if (!_transitions.ContainsKey(_currentState))
        {
            _transitions[_currentState] = new List<Transition>();
        }        
        _transitions[_currentState].Add(transition);
        _currentState = target;
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
        Transition transition = new Transition(target, (ch) => ch >= start && ch <= end, $"ch >= '{start}' && ch <= '{end}'");
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
        _states[id] = state;
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
        var cond = String.Join(" || ", ranges.Select(x => $"ch >= '{x.start}' && ch <= '{x.end}'"));

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

    public int ExceptTransition(char except)
    {
        int id = GetNewState();
        State state = new State(id);
        _states[id] = state;
        return ExceptTransitionTo(state.Id, except);
    }

    public int ExceptTransitionTo(string targetStateName, char except)
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

    public int ExceptTransitionTo(int target, char except)
    {
        var newStateId = GetNewState();
        State state = new State(newStateId);
        _states[newStateId] = state;
        Transition transition = new Transition(newStateId, (ch) => ch != except, $"ch != '{except}'");
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

    internal void AddKeyword(string arg0, string name)
    {
        _keywords[arg0] = name;
    }

    internal void AddFactory(string tokenName, string factory)
    {
        if (!_factories.ContainsKey(tokenName)) 
        {
            _factories[tokenName] = factory;
        }
    }

    #endregion


}

