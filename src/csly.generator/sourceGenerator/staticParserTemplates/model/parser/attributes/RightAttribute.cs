using System;

namespace <#NS#>;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class RightAttribute : InfixAttribute
{
    public RightAttribute(int intToken,  int precedence) : base(intToken,Associativity.Right,precedence)
    {
    }
        
    public RightAttribute(string stringToken,  int precedence) : base(stringToken,Associativity.Right, precedence)
    {
    }
}