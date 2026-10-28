[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CleanLambdaFunction.Presentation.LambdaFunction;

using Amazon.Lambda.Core;
using Application;
using Application.Authors.Queries.GetAuthors;
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
        await using var scope = this.provider.CreateAsyncScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var authors = await sender.Send(new GetAuthorsQuery());

        return [.. authors.Select(a => this.mapper.ToPresentation(a))];
    }
}
