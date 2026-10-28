# Clean Lambda Function

[![Build Pipeline](https://img.shields.io/github/actions/workflow/status/stphnwlsh/CleanLamdaFunction/build-pipeline.yml?branch=main&label=Build%20Pipeline&logo=github&style=for-the-badge)](https://github.com/stphnwlsh/CleanLamdaFunction/actions/workflows/build-pipeline.yml)
[![Code Coverage](https://img.shields.io/codecov/c/github/stphnwlsh/CleanLamdaFunction?label=Code%20Coverage&logo=codecov&logoColor=white&style=for-the-badge)](https://codecov.io/gh/stphnwlsh/CleanLamdaFunction)
[![OpenSSF Scorecard](https://img.shields.io/ossf-scorecard/github.com/stphnwlsh/CleanLamdaFunction?label=OpenSSF%20Scorecard&style=for-the-badge)](https://scorecard.dev/viewer/?uri=github.com/stphnwlsh/CleanLamdaFunction)
[![NuGet](https://img.shields.io/nuget/v/CleanLambdaFunction.Template?label=NuGet&logo=nuget&logoColor=white&style=for-the-badge)](https://www.nuget.org/packages/CleanLambdaFunction.Template)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CleanLambdaFunction.Template?label=Downloads&logo=nuget&logoColor=white&style=for-the-badge)](https://www.nuget.org/packages/CleanLambdaFunction.Template)
[![License](https://img.shields.io/github/license/stphnwlsh/CleanLamdaFunction?label=License&style=for-the-badge)](LICENSE.md)
[![GitHub Sponsors](https://img.shields.io/github/sponsors/stphnwlsh?label=Sponsor&logo=githubsponsors&logoColor=white&color=ea4aaa&style=for-the-badge)](https://github.com/sponsors/stphnwlsh)

A `dotnet new` template for an [AWS Lambda function](https://docs.aws.amazon.com/lambda/latest/dg/lambda-csharp.html) built with a streamlined version of Clean Architecture, packaged as a container image.

It's one of five templates that share the same Application and Infrastructure layers and differ only in how they present them. See [Related templates](#related-templates).

## Contents

- [Getting started](#getting-started)
- [Running the project](#running-the-project)
- [Architecture](#architecture)
- [Features](#features)
- [Related templates](#related-templates)
- [Resources](#resources)
- [Contributing](#contributing)
- [Support](#support)
- [License](#license)

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop) (optional, to build and run the container image)

### Install the template

``` bash
dotnet new install CleanLambdaFunction.Template
```

### Create a solution

``` bash
dotnet new cleanlambdafunction --name {YOUR_SOLUTION_NAMESPACE} --authorName "{YOUR_NAME}"
```

### Securing the function

The function is invoked directly, so there is no bearer token to check. AWS IAM decides who can call it: only principals allowed `lambda:InvokeFunction` on the function (through an IAM policy or the function's resource-based policy) can invoke it. To call it over HTTP with bearer tokens instead, put it behind an [API Gateway HTTP API with a JWT authorizer](https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api-jwt-authorizer.html), which validates the token before the function runs.

## Running the project

``` bash
docker build . --target run --tag cleanlamdafunction:local
docker run --rm -p 9000:8080 cleanlamdafunction:local
curl -X POST "http://localhost:9000/2015-03-31/functions/function/invocations" -d '{"query":"the","offset":0,"limit":5}'
```

The image runs the `GetMovies` function by default. The same handler also has `GetAuthors` and `GetReviews`. Each returns one page of results with a `TotalCount` and a `NextOffset`. Send the `NextOffset` back as `offset` to get the next page. Pass the function name to `docker run` to try another one.

``` bash
docker run --rm -p 9000:8080 cleanlamdafunction:local "Presentation::CleanLambdaFunction.Presentation.LambdaFunction.Handler::GetReviews"
curl -X POST "http://localhost:9000/2015-03-31/functions/function/invocations" -d '{"movieId":"3fa85f64-5717-4562-b3fc-2c963f66afa6","limit":5}'
```

`GetReviews` takes an optional `movieId` and `authorId` in place of `query`. The function keeps its SQLite database in `/tmp`, the only place Lambda can write, and stops any work that would outrun the invocation's remaining time.

The `Dockerfile` has stages for testing, publishing and running, so you can build just the part you need. You can also stamp a version into the assembly.

``` bash
docker build . --target run --tag cleanlamdafunction:latest --build-arg VERSION_PREFIX={VERSION_NUMBER} --build-arg VERSION_SUFFIX={PRERELEASE_NAME}
```

## Architecture

This solution is loosely based on Clean Architecture. It's by no means perfect, so I prefer to call it "Lean Mean Clean Architecture". Inspiration comes from [Jason Taylor's Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture), with some ideas taken further and others scaled back.

There's a little CQRS in the style of the commands and queries, but reads and writes use the same data source underneath.

The one deliberate break from Clean Architecture is that the Presentation project references the Infrastructure project, so it can wire up dependency injection. To protect that boundary, every class in Infrastructure is `internal`, so it can't be used from Presentation by accident.

### Project structure

1. **Presentation** - Exposes the Application layer as an AWS Lambda handler. The handler builds the service provider once per container, opens a new scope for each invocation, sends a query through Mediator and maps the result into its response model.
1. **Application** - Owns the domain and business logic. Commands and queries are validated and handled here, and each domain type has its own repository interface. This layer doesn't care what fulfils those contracts, as long as something does.
1. **Infrastructure** - Owns the data. It implements the repository interfaces to fetch, create, update and remove data, and maps its own data models onto the Application entities so persistence details don't leak upwards.

## Features

- Serverless functions using [AWS Lambda for .NET](https://github.com/aws/aws-lambda-dotnet), deployed as a container image
- JSON using the source-generated Lambda serializer, which avoids reflection on cold start
- Mediator pattern using [Mediator](https://github.com/martinothamar/Mediator)
- Offset paging and search for movies, authors and reviews
- Validation using [FluentValidation](https://github.com/FluentValidation/FluentValidation) in an Application pipeline behaviour
- Object mapping using [Mapperly](https://mapperly.riok.app)
- Data stored in [SQLite](https://learn.microsoft.com/ef/core/providers/sqlite/) using [Entity Framework Core](https://learn.microsoft.com/ef/core) migrations, seeded with [Bogus](https://github.com/bchavez/Bogus). Tests use a throwaway in-memory SQLite database, so they never touch a real one
- Testing using [xUnit](https://xunit.net), [Shouldly](https://github.com/shouldly/shouldly) and [NSubstitute](https://github.com/nsubstitute/NSubstitute), with coverage from [Coverlet](https://github.com/coverlet-coverage/coverlet)
- CI with GitHub Actions: build and test, 95% coverage gate, [Trivy](https://trivy.dev) container scan, [CodeQL](https://codeql.github.com) static analysis, then NuGet publishing and a GitHub release from `main` using [trusted publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing)
- Monthly grouped dependency updates using [Dependabot](https://docs.github.com/code-security/dependabot)

## Related templates

All five templates share the same Application and Infrastructure layers, tests, CI pipeline and repository setup.

| Template | Presentation | Package |
| --- | --- | --- |
| [Clean Minimal API](https://github.com/stphnwlsh/CleanMinimalApi) | ASP.NET Core Minimal APIs | [CleanMinimalApi.Template](https://www.nuget.org/packages/CleanMinimalApi.Template) |
| [Clean Web API](https://github.com/stphnwlsh/CleanWebApi) | ASP.NET Core controllers | [CleanWebApi.Template](https://www.nuget.org/packages/CleanWebApi.Template) |
| [Clean GraphQL API](https://github.com/stphnwlsh/CleanGraphQLApi) | GraphQL | [CleanGraphQLApi.Template](https://www.nuget.org/packages/CleanGraphQLApi.Template) |
| [Clean Model Context Protocol](https://github.com/stphnwlsh/CleanModelContextProtocol) | Model Context Protocol server | [CleanModelContextProtocol.Template](https://www.nuget.org/packages/CleanModelContextProtocol.Template) |
| **Clean Lambda Function** (this repo) | AWS Lambda function | [CleanLambdaFunction.Template](https://www.nuget.org/packages/CleanLambdaFunction.Template) |

## Resources

This template wouldn't exist without these resources. If you're on your own learning adventure, they're well worth a read.

- [AWS - Building Lambda functions with C#](https://docs.aws.amazon.com/lambda/latest/dg/lambda-csharp.html)
- [AWS - Testing Lambda container images locally](https://docs.aws.amazon.com/lambda/latest/dg/images-test.html)
- [Coder Jony - Serilog in AWS Lambda using .NET Core](https://coderjony.com/blogs/serilog-in-aws-lambda-using-net-core/)
- [Jason Taylor - Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture)
- [Robert C. Martin - The Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## Contributing

Bug reports, ideas and pull requests are welcome. Please read the [contributing guide](CONTRIBUTING.md) and the [code of conduct](CODE_OF_CONDUCT.md) first. To report a security vulnerability, follow the [security policy](SECURITY.md) instead of opening an issue.

## Support

If this template saves you time, please consider [sponsoring me on GitHub](https://github.com/sponsors/stphnwlsh). It helps me keep all five templates up to date.

[![Please sponsor me](docs/sponsor.jpg)](https://github.com/sponsors/stphnwlsh)

You can also connect with me on [LinkedIn](https://www.linkedin.com/in/stphnwlsh), follow me on [Medium](https://stphnwlsh.medium.com) or [GitHub](https://github.com/stphnwlsh). For help using the template, see [SUPPORT.md](SUPPORT.md).

## License

Released under the [MIT License](LICENSE.md).
