[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CleanLambdaFunction.Presentation.LambdaFunction;

using Amazon.Lambda.Core;
using Application;
using Application.Authors.Queries.GetAuthors;
using Infrastructure;
using Mapping;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Models;

public class Handler
{
    private readonly IMediator mediator;
    private readonly LambdaFunctionMapper mapper = new();

    public Handler()
    {
        var services = new ServiceCollection();

        _ = services.AddInfrastructure();
        _ = services.AddApplication();

        var provider = services.BuildServiceProvider();

        this.mediator = provider.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<List<Author>> Handle(string input, ILambdaContext context)
    {
        var authors = await this.mediator.Send(new GetAuthorsQuery());

        return [.. authors.Select(a => this.mapper.ToPresentation(a))];
    }
}
