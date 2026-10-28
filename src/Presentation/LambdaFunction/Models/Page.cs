namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

/// <summary>
/// One page of results.
/// </summary>
/// <typeparam name="T">The type of the results.</typeparam>
/// <param name="Items">The results on this page.</param>
/// <param name="TotalCount">The number of results across every page.</param>
/// <param name="NextOffset">The offset that fetches the next page, or null when this is the last page.</param>
public record Page<T>(List<T> Items, int TotalCount, int? NextOffset);
