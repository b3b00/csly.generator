namespace generatorTests;

public class ExprTests
{
    
    [Fact]
    public void TestExpr()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain(instance);
        var result = main.Parse("3 + 5 - 2 * 4 + 10 / 2");
        Assert.True(result.IsOk);
        Assert.Equal(6, result.Result);
    }

}
