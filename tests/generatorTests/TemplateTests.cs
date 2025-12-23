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
}
