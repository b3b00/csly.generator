using benchgenerator.csly;
using benchgenerator.csly.JsonModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using json;
using json.jsonparser;
using sly.parser.generator;
using JsonTokenGeneric = benchgenerator.csly.JsonTokenGeneric;

namespace benchgenerator;



[SimpleJob(RuntimeMoniker.Net90)]
[RPlotExporter]
[MemoryDiagnoser]
[ShortRunJob] 
[BaselineColumn]
public class BenchJson
{
    private JsonParserMain generatedParser;
    private sly.parser.Parser<JsonTokenGeneric, JSon> cslyParser;
    private string json;

    [GlobalSetup]
    public void Setup()
    {
        JsonParser instance =  new JsonParser();
        generatedParser = new JsonParserMain(instance);
        json = File.ReadAllText("big.json");

        var builder = new ParserBuilder<JsonTokenGeneric, JSon>();
        var r = builder.BuildParser(new EbnfJsonGenericParser(), ParserType.EBNF_LL_RECURSIVE_DESCENT, "root");
        if (r.IsOk)
        {
            cslyParser = r.Result;
        }
        else
        {
            throw new Exception($"could not build csly json parser {string.Join(", ",r.Errors.Select(x => x.Message))}"); 
        }
    }

    [Benchmark]
    public void Generator()
    {
        generatedParser.Parse(json);
    }

    [Benchmark(Baseline = true)]
    public void Csly()
    {
        cslyParser.Parse(json);
    }
}