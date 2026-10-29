[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.SourceGeneratorLambdaJsonSerializer<CleanLambdaFunction.Presentation.LambdaFunction.Serialization.LambdaFunctionJsonSerializerContext>))]

namespace CleanLambdaFunction.Presentation.LambdaFunction;

using Amazon.Lambda.Core;
using Application;
using Application.Authors.Queries.GetAuthors;
using Application.Movies.Queries.GetMovies;
using Application.Reviews.Queries.GetReviews;
using Infrastructure;
using Mapping;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Models;
using AppPaging = Application.Common.Paging;

public class Handler
{
    /// <summary>
    /// Lambda can only write to /tmp, so the SQLite database lives there unless the environment variable says otherwise.
    /// </summary>
    public const string DefaultConnectionString = "Data Source=/tmp/movie-reviews.db";

    public const string ConnectionStringVariable = "ConnectionStrings__MovieReviews";

    /// <summary>
    /// Set it to true to seed sample data into an empty database. Leave it unset anywhere real data lives.
    /// </summary>
    public const string SeedSampleDataVariable = "Database__SeedSampleData";

    private readonly ServiceProvider provider;
    private readonly LambdaFunctionMapper mapper = new();

    public Handler()
        : this(ConnectionStringFromEnvironment(), SeedSampleDataFromEnvironment())
    {
    }

    public Handler(string connectionString, bool seedSampleData)
    {
        var services = new ServiceCollection();

        _ = services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        _ = services.AddInfrastructure(connectionString);
        _ = services.AddApplication();

        this.provider = services.BuildServiceProvider(validateScopes: true);
        _ = this.provider.InitialiseInfrastructure(seedSampleData);
    }

    /// <summary>
    /// Lists movies ordered by title, one page at a time, optionally only those whose title contains a search term.
    /// This is the function the image runs by default.
    /// </summary>
    /// <param name="request">The search term, offset and limit.</param>
    /// <param name="context">The Lambda invocation context.</param>
    /// <returns>One page of movies, each with its review count and average stars.</returns>
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
    /// <returns>One page of authors, each with their review count and average stars.</returns>
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

    /// <summary>
    /// The connection string from the environment, or the default in /tmp when it isn't set.
    /// </summary>
    /// <returns>The connection string the handler uses.</returns>
    public static string ConnectionStringFromEnvironment()
    {
        return Environment.GetEnvironmentVariable(ConnectionStringVariable) ?? DefaultConnectionString;
    }

    /// <summary>
    /// Whether the environment asks for sample data. Anything other than true, including no value, means no.
    /// </summary>
    /// <returns>True when sample data should be seeded into an empty database.</returns>
    public static bool SeedSampleDataFromEnvironment()
    {
        return bool.TryParse(Environment.GetEnvironmentVariable(SeedSampleDataVariable), out var seedSampleData) && seedSampleData;
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
