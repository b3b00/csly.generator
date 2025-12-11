namespace generatorTests;

public class ExprTests
{
    
    [Fact]
    public void TestExpr()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain(instance);
        var result = main.Parse("3 + 5 - 2 * 4 + 10 / 2");
        //all operations are right associative so result is 3 + (5 - (2 * (4 + (10 / 2)))) = -5
        int expected = (3 + (5 - ((2 * 4) + (10 / 2))));
        Assert.True(result.IsOk);
        Assert.Equal(expected, result.Result);
    }

}
