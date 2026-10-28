namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

using Application.Common.Paging;

/// <summary>
/// Searches movies by title or authors by name, one page at a time.
/// </summary>
public record SearchRequest
{
    /// <summary>The words to look for, ignoring case. Leave empty to list every one.</summary>
    public string Query { get; init; }

    /// <summary>The largest number of results to return, from 1 to 50.</summary>
    public int Limit { get; init; } = Paging.DefaultLimit;

    /// <summary>The NextCursor from a previous page, or empty for the first page.</summary>
    public string Cursor { get; init; }
}
