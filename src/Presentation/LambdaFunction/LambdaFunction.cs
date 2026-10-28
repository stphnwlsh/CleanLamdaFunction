[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.SourceGeneratorLambdaJsonSerializer<CleanLambdaFunction.Presentation.LambdaFunction.Serialization.LambdaFunctionJsonSerializerContext>))]

namespace CleanLambdaFunction.Presentation.LambdaFunction;

using Amazon.Lambda.Core;
using Application;
using Application.Authors.Queries.GetAuthors;
using AppPaging = Application.Common.Paging;
using Application.Movies.Queries.GetMovies;
using Application.Reviews.Queries.GetReviews;
using Infrastructure;
using Mapping;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Models;

public class Handler
{
    /// <summary>
    /// Lambda can only write to /tmp, so the SQLite database lives there unless the environment variable says otherwise.
    /// </summary>
    public const string DefaultConnectionString = "Data Source=/tmp/movie-reviews.db";

    public const string ConnectionStringVariable = "ConnectionStrings__MovieReviews";

    private readonly ServiceProvider provider;
    private readonly LambdaFunctionMapper mapper = new();

    public Handler()
        : this(Environment.GetEnvironmentVariable(ConnectionStringVariable) ?? DefaultConnectionString)
    {
    }

    public Handler(string connectionString)
    {
        var services = new ServiceCollection();

        _ = services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        _ = services.AddInfrastructure(connectionString);
        _ = services.AddApplication();

        this.provider = services.BuildServiceProvider(validateScopes: true);
        _ = this.provider.InitialiseInfrastructure();
    }

    /// <summary>
    /// Lists movies ordered by title, one page at a time, optionally only those whose title contains a search term.
    /// This is the function the image runs by default.
    /// </summary>
    /// <param name="request">The search term, offset and limit.</param>
    /// <param name="context">The Lambda invocation context.</param>
    /// <returns>One page of movies.</returns>
    public async Task<Page<Movie>> GetMovies(ListRequest request, ILambdaContext context)
    {
        var page = await this.Send(new GetMoviesQuery
        {
            Query = request.Query,
            Offset = request.Offset,
            Limit = request.Limit
        }, context);

        return ToPage(page, this.mapper.ToPresentation);
    }

    /// <summary>
    /// Lists authors ordered by last name then first name, one page at a time, optionally only those whose name contains a search term.
    /// </summary>
    /// <param name="request">The search term, offset and limit.</param>
    /// <param name="context">The Lambda invocation context.</param>
    /// <returns>One page of authors.</returns>
    public async Task<Page<Author>> GetAuthors(ListRequest request, ILambdaContext context)
    {
        var page = await this.Send(new GetAuthorsQuery
        {
            Query = request.Query,
            Offset = request.Offset,
            Limit = request.Limit
        }, context);

        return ToPage(page, this.mapper.ToPresentation);
    }

    /// <summary>
    /// Lists reviews ordered by movie then author, one page at a time, optionally only those of one movie, by one author, or both.
    /// </summary>
    /// <param name="request">The movie, author, offset and limit.</param>
    /// <param name="context">The Lambda invocation context.</param>
    /// <returns>One page of reviews.</returns>
    public async Task<Page<Review>> GetReviews(ListReviewsRequest request, ILambdaContext context)
    {
        var page = await this.Send(new GetReviewsQuery
        {
            MovieId = request.MovieId,
            AuthorId = request.AuthorId,
            Offset = request.Offset,
            Limit = request.Limit
        }, context);

        return ToPage(page, this.mapper.ToPresentation);
    }

    private static Page<TResult> ToPage<T, TResult>(AppPaging.Page<T> page, Func<T, TResult> map)
    {
        var nextOffset = page.Offset + page.Items.Count;

        return new Page<TResult>([.. page.Items.Select(map)], page.TotalCount, nextOffset < page.TotalCount ? nextOffset : null);
    }

    private async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, ILambdaContext context)
    {
        // Stop the work when the invocation runs out of time, rather than letting Lambda kill it part way through.
        using var cancellation = context.RemainingTime > TimeSpan.Zero
            ? new CancellationTokenSource(context.RemainingTime)
            : new CancellationTokenSource();

        await using var scope = this.provider.CreateAsyncScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        return await sender.Send(request, cancellation.Token);
    }
}
