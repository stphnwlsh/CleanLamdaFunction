namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

using Application.Common.Paging;

/// <summary>
/// Searches reviews, optionally only those of one movie or by one author, one page at a time.
/// </summary>
public record SearchReviewsRequest
{
    /// <summary>Only return reviews of this movie. Leave empty for every movie.</summary>
    public Guid? MovieId { get; init; }

    /// <summary>Only return reviews by this author. Leave empty for every author.</summary>
    public Guid? AuthorId { get; init; }

    /// <summary>The largest number of results to return, from 1 to 50.</summary>
    public int Limit { get; init; } = Paging.DefaultLimit;

    /// <summary>The NextCursor from a previous page, or empty for the first page.</summary>
    public string Cursor { get; init; }
}
