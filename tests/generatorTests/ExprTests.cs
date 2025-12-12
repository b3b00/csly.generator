using NFluent;

namespace generatorTests;

public class ExprTests
{
    
    [Fact]
    public void TestRightAssocExpr()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain(instance);
        var result = main.Parse("3 + 5 - 2 * 4 + 10 / 2");
        //all operations are right associative so result is 3 + (5 - (2 * (4 + (10 / 2)))) = -5
        int expected = (3 + (5 - ((2 * 4) + (10 / 2))));
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsEqualTo(expected);
    }

    [Fact]
    public void TestLefttAssocExpr()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain(instance);
        var result = main.Parse("3 +l 5 -l 2 *l 4 +l 10 /l 2");
        //all operations are left associative so result is (((3 + 5) - 2) * 4) + 10) / 2 = 10
        int expected = ((3 + 5) - ((2 * 4) + (10 / 2)));
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsEqualTo(expected);
    }

    [Fact]
    public void TestGroupedExpr()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain(instance);
        var result = main.Parse("( 3 + 5 ) * ( 10 - 2 )");
        //grouped operations (3 + 5) * (10 - 2) = 64
        int expected = (3 + 5) * (10 - 2);
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsEqualTo(expected);
    }

    
    [Fact]
    public void TestSingleInt()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain(instance);
        var result = main.Parse("42");
        int expected = 42;
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsEqualTo(expected);
    }

    // https://github.com/b3b00/csly/issues/164
    [Fact]
    public void TestCslyIssue164()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain(instance);
        var result = main.Parse("1(1");
        
        Check.That(result.IsError).IsTrue();
        var errors = result.Errors;
        Check.That(errors).CountIs(1);

    }


}
