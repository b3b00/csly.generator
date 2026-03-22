using System;
using System.Collections.Generic;
using System.Linq;

namespace csly.generator.model.lexer;

public class LexerIndentation
{
    private IList<string> _indentations = new List<string>() {""};
    private int _currentLevel = 0;

    public int CurrentLevel => _currentLevel;

    public string Current
    {
        get
        {
            if (_indentations.Count > 0 && _currentLevel < _indentations.Count && _currentLevel >= 0)
            {
                return _indentations[_currentLevel];
            }
            return "";
        }
    }

    private void DoIndent(string shift)
    {
        if (!_indentations.Contains(shift))
        {
            _indentations.Add(shift);
            // Sort by length using insertion sort
            var sorted = new List<string>(_indentations.Count);
            foreach (var indent in _indentations)
            {
                int insertIndex = sorted.Count;
                for (int i = 0; i < sorted.Count; i++)
                {
                    if (indent.Length < sorted[i].Length)
                    {
                        insertIndex = i;
                        break;
                    }
                }
                sorted.Insert(insertIndex, indent);
            }
            _indentations = sorted;
        }

        _currentLevel ++;
    } 
        
    private void DoUindent()
    {
        _currentLevel = Math.Max(0,_currentLevel-1);
    }
    public (bool isIndent, LexerIndentationType type) Indent(string shift)
    {
        if (_indentations.Count == 0)
        {
            if (shift.Length > 0)
            {
                DoIndent(shift);
                return (true, LexerIndentationType.Indent);
            }

            return (true, LexerIndentationType.None);
        }
        else
        {
            if (IsError(shift))
            {
                return (false, LexerIndentationType.Error);
            } 
            if (shift.Length > Current.Length)
            {
                DoIndent(shift);
                return (true, LexerIndentationType.Indent);
            }
            if (shift.Length < Current.Length)
            {
                DoUindent();
                return (true, LexerIndentationType.UIndent);
            }

            if (shift.Length == Current.Length)
            {
                return (true, LexerIndentationType.None);
            }
                
        }
            
        return (true,LexerIndentationType.None);
    }

    private bool IsError(string shift)
    {
        // indent case : shift must match all previous indentations
        if (shift.Length > _indentations[_indentations.Count-1].Length)
        {
            for (int i = 0; i < _indentations.Count; i++)
            {
                if (!shift.StartsWith(_indentations[i]))
                {
                    return true;
                }
            }
            return false;
        }

        if (shift.Length == 0)
        {
            return false;
        }
        var level = _indentations.IndexOf(shift);
        return level < 0;
    }

    public LexerIndentation Clone()
    {
        var clonedIndentations = new List<string>(_indentations.Count);
        for (int i = 0; i < _indentations.Count; i++)
        {
            clonedIndentations.Add(_indentations[i]);
        }
        return new LexerIndentation()
        {
            _currentLevel = _currentLevel,
            _indentations = clonedIndentations
        };
    }
        
        
}