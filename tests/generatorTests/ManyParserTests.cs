using manyParser;
using manyParser.parser1;
using manyParser.parser2;
using NFluent;

namespace generatorTests;

public class ManyParserTests
{

   [Fact]
   public void Parser1Test()
   {
      Parser1 p1 = new Parser1();
      Parser1Main parser = new Parser1Main(p1);

      var parsed = parser.Parse("one 1 2 3 4 5");
      Check.That(parsed.IsOk).IsTrue();
      Check.That(parsed.Result).IsEqualTo("one : 1 2 3 4 5");

   }

   [Fact]
   public void Parser2Test()
   {
      Parser2 p2 = new Parser2();
      Parser2Main parser = new Parser2Main(p2);
      var parsed = parser.Parse("two 2 4 6 8 10");
      Check.That(parsed.IsOk).IsTrue();
      Check.That(parsed.Result).IsEqualTo("two : 2 4 6 8 10");
   }

   [Fact]
   public void BothParserTest()
   {
      
   }


}
