namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

using Application.Common.Paging;

/// <summary>
/// Lists reviews one page at a time, optionally only those of one movie, by one author, or both.
/// </summary>
public record ListReviewsRequest
{
    /// <summary>Only return reviews of this movie. Leave empty for every movie.</summary>
    public Guid? MovieId { get; init; }

    /// <summary>Only return reviews by this author. Leave empty for every author.</summary>
    public Guid? AuthorId { get; init; }

    /// <summary>The number of results to skip, 0 for the first page. Use the NextOffset from a previous page to get the next one.</summary>
    public int Offset { get; init; }

    /// <summary>The largest number of results to return, from 1 to 50.</summary>
    public int Limit { get; init; } = Paging.DefaultLimit;
}
