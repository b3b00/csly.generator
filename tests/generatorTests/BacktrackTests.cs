using Xunit;
using backtrack;
using NFluent;


namespace ParserTests.samples;

[CollectionDefinition("backtrack", DisableParallelization = true)]
public class BacktrackTests
{



    public BackTrackParserMain BuildParser()
    {
        var parser = new BackTrackParserMain(new BackTrackParser(), useMemoization: true);
        return parser;
    }

    [Fact]
    public void Parse_Backtrack()
    {
        var parser = BuildParser(); 
        var result = parser.Parse("funA(funB(C == 2));");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsEqualTo("funA(funB(C==2));");
    }


    
}
