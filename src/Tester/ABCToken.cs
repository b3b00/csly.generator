using csly.Tester.testerparser.models;

namespace csly.generator.tester;

public enum ABCToken
{
    [AlphaId]
    ID, 
    
    [Keyword("A")]
    A,
    [Keyword("B")]
    B,
    [Keyword("C")]
    C,  
}