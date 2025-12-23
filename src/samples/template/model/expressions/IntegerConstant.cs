using System;
using System.Collections.Generic;
using template.model;
using csly.template.models;

namespace template.model.expressions
{
    public class IntegerConstant : Expression
    {
        public IntegerConstant(int value, LexerPosition position)
        {
            Value = value;
            Position = position;
        }

        public int Value { get; set; }

        public LexerPosition Position { get; set; }


        public string Dump(string tab)
        {
            return $"{tab}(INTEGER {Value})";
        }


        public string GetValue(Dictionary<string, object> context)
        {
            return Value.ToString();
        }

        public object Evaluate(Dictionary<string, object> context)
        {
            return Value;
        }
    }
}