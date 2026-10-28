namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

using Application.Common.Paging;

/// <summary>
/// Lists movies or authors one page at a time, optionally only those that match a search term.
/// </summary>
public record ListRequest
{
    /// <summary>The words to look for, ignoring case. Leave empty to list every one.</summary>
    public string? Query { get; init; }

    /// <summary>The number of results to skip, 0 for the first page. Use the NextOffset from a previous page to get the next one.</summary>
    public int Offset { get; init; }

    /// <summary>The largest number of results to return, from 1 to 50.</summary>
    public int Limit { get; init; } = Paging.DefaultLimit;
}
