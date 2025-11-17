using System;

namespace csly.models;

public interface ISyntaxNode<IN, OUT> where IN : struct, Enum
{

    public bool IsEpsilon { get;}
    
    bool Discarded { get;  }
    string Name { get; }
    
    bool HasByPassNodes { get; set; }
    
    string Dump(string tab);

    string ToJson(int index = 0);
    
    void ForceName(string name);
    
}