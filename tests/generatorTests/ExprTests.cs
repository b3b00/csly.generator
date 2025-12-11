namespace generatorTests;

public class ExprTests
{
    
    [Fact]
    public void TestExpr()
    {
        ExprParser instance = new ExprParser();
        ExprParserMain main = new ExprParserMain();
        var result = main.Parse(instance, "3 + 5 - 2 * 4 + 10 / 2");
        Assert.False(result.HasErrors);
        Assert.Equal(6, result.Result);
    }

}
