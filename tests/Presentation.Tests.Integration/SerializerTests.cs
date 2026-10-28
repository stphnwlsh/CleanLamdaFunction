namespace CleanLambdaFunction.Presentation.Tests.Integration;

using System.Text;
using Amazon.Lambda.Serialization.SystemTextJson;
using CleanLambdaFunction.Presentation.LambdaFunction.Models;
using CleanLambdaFunction.Presentation.LambdaFunction.Serialization;
using Shouldly;
using Xunit;

public class SerializerTests
{
    private readonly SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext> serializer = new();

    [Fact]
    public void Deserialize_ShouldRead_ListRequest()
    {
        // Arrange
        using var input = new MemoryStream(Encoding.UTF8.GetBytes("""{"query":"star","offset":20,"limit":5}"""));

        // Act
        var request = this.serializer.Deserialize<ListRequest>(input);

        // Assert
        request.Query.ShouldBe("star");
        request.Offset.ShouldBe(20);
        request.Limit.ShouldBe(5);
    }

    [Fact]
    public void Serialize_ShouldWrite_Page_WithoutNulls()
    {
        // Arrange
        using var output = new MemoryStream();
        var page = new Page<Movie>([new Movie(Guid.Empty, "Lorem Ipsum")], 1, null);

        // Act
        this.serializer.Serialize(page, output);

        // Assert
        Encoding.UTF8.GetString(output.ToArray()).ShouldBe("""{"Items":[{"Id":"00000000-0000-0000-0000-000000000000","Title":"Lorem Ipsum"}],"TotalCount":1}""");
    }
}
