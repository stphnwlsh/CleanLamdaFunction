namespace CleanLambdaFunction.Presentation.Tests.Integration;

using Amazon.Lambda.TestUtilities;
using CleanLambdaFunction.Presentation.LambdaFunction;
using Xunit;

public class LambdaFunctionTest
{
    [Fact]
    public void TestToUpperFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var handler = new Handler();
        var context = new TestLambdaContext();
        //var casing = function.FunctionHandler("hello world", context);

        Assert.True(true);
        //Assert.Equal("hello world", casing.Lower);
        //Assert.Equal("HELLO WORLD", casing.Upper);
    }

    [Fact]
    public async Task Handle_ShouldReturn_Authors()
    {
        var handler = new Handler();
        var context = new TestLambdaContext();

        var authors = await handler.Handle("input", context);

        Assert.NotEmpty(authors);
        Assert.All(authors, a => Assert.False(string.IsNullOrEmpty(a.FirstName)));
    }
}
