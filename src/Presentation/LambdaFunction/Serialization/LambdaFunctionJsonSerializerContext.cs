namespace CleanLambdaFunction.Presentation.LambdaFunction.Serialization;

using System.Text.Json.Serialization;
using Models;

/// <summary>
/// Source-generated JSON metadata for every request and response, so the function doesn't use reflection to serialise.
/// </summary>
[JsonSerializable(typeof(ListRequest))]
[JsonSerializable(typeof(ListReviewsRequest))]
[JsonSerializable(typeof(Page<Movie>))]
[JsonSerializable(typeof(Page<Author>))]
[JsonSerializable(typeof(Page<Review>))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
}
