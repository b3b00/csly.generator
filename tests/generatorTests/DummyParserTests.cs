using NFluent;
using Tester;
using Tester.dummyparser;

namespace generatorTests;




public class DummyParserTests
{
    
    [Fact]
    public void ABCTest()
    {
        var instance = new DummyParser();
        DummyParserMain main = new DummyParserMain(instance);
        
        var r = main.Parse("A B C");
        Check.That(r.IsOk).IsTrue();
        Check.That(r.Result).IsEqualTo($"a=A b=B c=C");
    }
    
    [Fact]
    public void BCTest()
    {
        var instance = new DummyParser();
        DummyParserMain main = new DummyParserMain(instance);
        
        var r = main.Parse("B C");
        Check.That(r.IsOk).IsTrue();
        Check.That(r.Result).IsEqualTo($"a=<none> b=B c=C");
    }
    
    [Fact]
    public void CTest()
    {
        var instance = new DummyParser();
        DummyParserMain main = new DummyParserMain(instance);
        
        var r = main.Parse("C");
        Check.That(r.IsOk).IsTrue();
        Check.That(r.Result).IsEqualTo($"a=<none> b=<none> c=C");
    }
}