using backtrack;
using backtrack.backtrackparser;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using json;
using json.jsonparser;
using sly.parser.generator;

namespace backtrackbench;



[SimpleJob(RuntimeMoniker.Net90)]
[RPlotExporter]
[MemoryDiagnoser]
[ShortRunJob] 
[BaselineColumn]
public class BenchBackTrack

{
    private BackTrackParserMain  generatedParser;
    private sly.parser.Parser<benchgenerator.csly.backtrack.BackTrackToken, string> cslyParser;
    private string source;

    [GlobalSetup]
    public void Setup()
    {
        BackTrackParser instance =  new BackTrackParser();
        generatedParser = new BackTrackParserMain(instance);
        source = "funA(funB(C == 2));";

        var builder = new ParserBuilder<benchgenerator.csly.backtrack.BackTrackToken, string>();
        var r = builder.BuildParser(new benchgenerator.csly.backtrack.BackTrackParser(), ParserType.EBNF_LL_RECURSIVE_DESCENT, "block");
        if (r.IsOk)
        {
            cslyParser = r.Result;
        }
        else
        {
            throw new Exception($"could not build csly backtrack parser {string.Join(", ",r.Errors.Select(x => x.Message))}"); 
        }
    }

    [Benchmark]
    public void Generator()
    {
        generatedParser.Parse(source);
    }

    [Benchmark(Baseline = true)]
    public void Csly()
    {
        cslyParser.Parse(source);
    }
}