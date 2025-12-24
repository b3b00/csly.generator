using NFluent;
using template;

namespace generatorTests;

[CollectionDefinition("template", DisableParallelization = true)]
public class TemplateTests 
{
    [Fact]
    public void TestTemplateParsing()
    {
        TemplateParser instance = new TemplateParser();
        TemplateParserMain main = new TemplateParserMain(instance);
        var result = main.Parse(@"hello-{=world=}-billy-{% if (a == 2) %}-bob-{%else%}-boubou-{%endif%}this is the end");
        Check.That(result.IsOk).IsTrue();
        var template = result.Result;
        var output = template.GetValue(new Dictionary<string, object>()
            {
                { "world", "WORLD" },
                { "a", 1 }
            });
        Check.That(output).IsEqualTo("hello-WORLD-billy--boubou-this is the end");
    }

    [Fact]
    public void TestTemplateParsing2()
    {
        TemplateParser instance = new TemplateParser();
        TemplateParserMain main = new TemplateParserMain(instance);
        var result = main.Parse(@"Numbers:{% for 1..10 as num %}-{=num=}-{% end %}The end.");
        Check.That(result.IsOk).IsTrue();
        var template = result.Result;
        var output = template.GetValue(new Dictionary<string, object>()
        {
            { "count",10 }
        });
        Check.That(output).IsEqualTo("Numbers:-1--2--3--4--5--6--7--8--9--10-The end.");
    }
}
