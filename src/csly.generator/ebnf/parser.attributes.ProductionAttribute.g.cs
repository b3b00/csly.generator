using System;

namespace csly.ebnf.models
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ProductionAttribute : Attribute
    {
        public ProductionAttribute(string rule)
        {
            RuleString = rule;
        }

        public string RuleString { get; set; }
    }
}