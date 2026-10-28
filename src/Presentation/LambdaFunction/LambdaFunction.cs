[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CleanLambdaFunction.Presentation.LambdaFunction;

using Amazon.Lambda.Core;
using Application;
using Application.Authors.Queries.GetAuthors;
using Application.Authors.Queries.SearchAuthors;
using Application.Movies.Queries.SearchMovies;
using Application.Reviews.Queries.SearchReviews;
using Infrastructure;
using Mapping;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Models;

public class Handler
{
    private readonly ServiceProvider provider;
    private readonly LambdaFunctionMapper mapper = new();

    public Handler()
    {
        var services = new ServiceCollection();

        _ = services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        _ = services.AddInfrastructure();
        _ = services.AddApplication();

        this.provider = services.BuildServiceProvider(validateScopes: true);
    }

    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<List<Author>> Handle(string input, ILambdaContext context)
    {
        var authors = await this.Send(new GetAuthorsQuery());

        return [.. authors.Select(a => this.mapper.ToPresentation(a))];
    }

    /// <summary>
    /// Finds movies whose title contains the search term, ordered by title, one page at a time.
    /// </summary>
    /// <param name="request">The search term, limit and cursor.</param>
    /// <param name="context">The Lambda invocation context.</param>
    /// <returns>One page of matching movies.</returns>
    public async Task<Page<Movie>> SearchMovies(SearchRequest request, ILambdaContext context)
    {
        var page = await this.Send(new SearchMoviesQuery
        {
            Query = request.Query,
            Limit = request.Limit,
            Cursor = request.Cursor
        });

        return new Page<Movie>([.. page.Items.Select(m => this.mapper.ToPresentation(m))], page.TotalCount, page.NextCursor);
    }

    /// <summary>
    /// Finds authors whose first name, last name or full name contains the search term, ordered by name, one page at a time.
    /// </summary>
    /// <param name="request">The search term, limit and cursor.</param>
    /// <param name="context">The Lambda invocation context.</param>
    /// <returns>One page of matching authors.</returns>
    public async Task<Page<Author>> SearchAuthors(SearchRequest request, ILambdaContext context)
    {
        var page = await this.Send(new SearchAuthorsQuery
        {
            Query = request.Query,
            Limit = request.Limit,
            Cursor = request.Cursor
        });

        return new Page<Author>([.. page.Items.Select(a => this.mapper.ToPresentation(a))], page.TotalCount, page.NextCursor);
    }

    /// <summary>
    /// Finds reviews, optionally only those of one movie, by one author, or both, one page at a time.
    /// </summary>
    /// <param name="request">The movie, author, limit and cursor.</param>
    /// <param name="context">The Lambda invocation context.</param>
    /// <returns>One page of matching reviews.</returns>
    public async Task<Page<Review>> SearchReviews(SearchReviewsRequest request, ILambdaContext context)
    {
        var page = await this.Send(new SearchReviewsQuery
        {
            MovieId = request.MovieId,
            AuthorId = request.AuthorId,
            Limit = request.Limit,
            Cursor = request.Cursor
        });

        return new Page<Review>([.. page.Items.Select(r => this.mapper.ToPresentation(r))], page.TotalCount, page.NextCursor);
    }

    private async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
    {
        await using var scope = this.provider.CreateAsyncScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        return await sender.Send(request);
    }
}
