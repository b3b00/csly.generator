using Xunit;
using csly.whileLang;
using csly.whileLang.model;
using NFluent;
using csly.whileLang.interpreter;
using csly.whileLang.compiler;
using XML;


namespace ParserTests.samples;

[CollectionDefinition("xml", DisableParallelization = true)]
public class XmlTests
{



    public MinimalXmlParserMain BuildParser()
    {
        MinimalXmlParser instance = new MinimalXmlParser();

        var parser = new MinimalXmlParserMain(instance);
        return parser;

    }


    [Fact]
    public void TestSingleRootWithContent()
    {
        var parser = BuildParser();
        var result = parser.Parse("<root>content</root>");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result.Trim()).IsEqualTo("tag(open (root, ), text(content), close(root))");
    }
    
    [Fact]
    public void TestSingleRootWithContentAndAttribute()
    {
        var parser = BuildParser();
        var result = parser.Parse("<root attr= \"42\">content</root>");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result.Trim()).IsEqualTo("tag(open (root, attr = 42), text(content), close(root))");
    }
    
    [Fact]
    public void TestSingleRootWithContentAndComment()
    {
        var parser = BuildParser();
        var result = parser.Parse("<root>content<!-- comment --></root>");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result.Trim()).IsEqualTo("tag(open (root, ), text(content),comment( comment ), close(root))");
    }
    
    [Fact]
    public void TestSingleRootWithContentAndProcessingInstruction()
    {
        var parser = BuildParser();
        var result = parser.Parse("<root>content<? pi do=\"it\" ?></root>");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
        Check.That(result.Result.Trim()).IsEqualTo("tag(open (root, ), text(content),pi(pi :: do = it), close(root))");
    }
    
    [Fact]
    public void TestBigThing()
    {
        var parser = BuildParser();
        var result = parser.Parse(@"
<?xml version=""1.0""?>
<customers>
   <customer id=""55000"">
      <name>Charter Group</name>
      <!-- address 1 -->
      <address>
         <street>100 Main</street>
         <city>Framingham</city>
         <state>MA</state>
         <zip>01701</zip>
      </address>
      <!-- address 2 -->
      <address>
         <street>720 Prospect</street>
         <city>Framingham</city>
         <state>MA</state>
         <zip>01701</zip>
      </address>
      <!-- address 3 -->
      <address>
         <street>120 Ridge</street>
         <state>MA</state>
         <zip>01760</zip>
      </address>
   </customer>
</customers>");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsNotNull();
    }

}
