namespace CleanLambdaFunction.Presentation.Tests.Integration;

using Amazon.Lambda.TestUtilities;
using CleanLambdaFunction.Presentation.LambdaFunction;
using CleanLambdaFunction.Presentation.LambdaFunction.Models;
using FluentValidation;
using Shouldly;
using Xunit;

public class SearchFunctionTests
{
    private readonly Handler handler = new();
    private readonly TestLambdaContext context = new();

    [Fact]
    public async Task SearchMovies_ShouldReturn_FirstPage()
    {
        // Act
        var result = await this.handler.SearchMovies(new SearchRequest(), this.context);

        // Assert
        result.Items.Count.ShouldBe(10);
        result.TotalCount.ShouldBe(50);
        result.NextCursor.ShouldNotBeNullOrWhiteSpace();
        result.Items.ShouldAllBe(m => m.Id != Guid.Empty && !string.IsNullOrWhiteSpace(m.Title));
    }

    [Fact]
    public async Task SearchMovies_ShouldReturn_NextPage()
    {
        // Arrange
        var first = await this.handler.SearchMovies(new SearchRequest { Limit = 20 }, this.context);

        // Act
        var result = await this.handler.SearchMovies(new SearchRequest { Limit = 40, Cursor = first.NextCursor }, this.context);

        // Assert
        result.Items.Count.ShouldBe(30);
        result.TotalCount.ShouldBe(50);
        result.NextCursor.ShouldBeNull();
        result.Items.Select(m => m.Id).Intersect(first.Items.Select(m => m.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchMovies_ShouldReturn_Matches()
    {
        // Arrange
        var title = (await this.handler.SearchMovies(new SearchRequest(), this.context)).Items[0].Title;

        // Act
        var result = await this.handler.SearchMovies(new SearchRequest { Query = title.ToUpperInvariant() }, this.context);

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchMovies_ShouldThrow_WhenLimitIsNotValid()
    {
        // Act
        var exception = await Should.ThrowAsync<ValidationException>(() => this.handler.SearchMovies(new SearchRequest { Limit = 0 }, this.context));

        // Assert
        exception.Errors.ShouldContain(e => e.PropertyName == "Limit" && e.ErrorMessage == "A limit must be between 1 and 50.");
    }

    [Fact]
    public async Task SearchAuthors_ShouldReturn_Matches()
    {
        // Arrange
        var lastName = (await this.handler.Handle("input", this.context))[0].LastName;

        // Act
        var result = await this.handler.SearchAuthors(new SearchRequest { Query = lastName.ToLowerInvariant() }, this.context);

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(a => $"{a.FirstName} {a.LastName}".Contains(lastName, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchAuthors_ShouldReturn_FirstPage()
    {
        // Arrange
        var authors = await this.handler.Handle("input", this.context);

        // Act
        var result = await this.handler.SearchAuthors(new SearchRequest { Limit = 5 }, this.context);

        // Assert
        result.Items.Count.ShouldBe(Math.Min(5, authors.Count));
        result.TotalCount.ShouldBe(authors.Count);
    }

    [Fact]
    public async Task SearchReviews_ShouldReturn_FirstPage()
    {
        // Act
        var result = await this.handler.SearchReviews(new SearchReviewsRequest(), this.context);

        // Assert
        result.Items.Count.ShouldBe(10);
        result.TotalCount.ShouldBe(150);
        result.NextCursor.ShouldNotBeNullOrWhiteSpace();
        result.Items.ShouldAllBe(r => r.Stars >= 1 && r.Stars <= 5 && r.ReviewedMovie != null);
    }

    [Fact]
    public async Task SearchReviews_ShouldReturn_ReviewsOfMovie()
    {
        // Arrange
        var movieId = (await this.handler.SearchReviews(new SearchReviewsRequest(), this.context)).Items[0].ReviewedMovie.Id;

        // Act
        var result = await this.handler.SearchReviews(new SearchReviewsRequest { MovieId = movieId, Limit = 50 }, this.context);

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(r => r.ReviewedMovie.Id == movieId);
    }

    [Fact]
    public async Task SearchReviews_ShouldReturn_ReviewsByAuthor()
    {
        // Arrange
        var author = (await this.handler.Handle("input", this.context)).First(a => a.Reviews.Count > 0);

        // Act
        var result = await this.handler.SearchReviews(new SearchReviewsRequest { AuthorId = author.Id, Limit = 50 }, this.context);

        // Assert
        result.TotalCount.ShouldBe(author.Reviews.Count);
        result.Items.Select(r => r.Id).ShouldBe(author.Reviews.Select(r => r.Id), ignoreOrder: true);
    }

    [Fact]
    public async Task SearchReviews_ShouldThrow_WhenMovieIdIsEmpty()
    {
        // Act
        var exception = await Should.ThrowAsync<ValidationException>(() =>
            this.handler.SearchReviews(new SearchReviewsRequest { MovieId = Guid.Empty }, this.context));

        // Assert
        exception.Errors.ShouldContain(e => e.PropertyName == "MovieId");
    }
}
