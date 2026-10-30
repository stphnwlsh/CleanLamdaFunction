[![Clean Lambda Function](https://raw.githubusercontent.com/LeanMeanCleanApps/brand/main/assets/banners/png/lambda.png)](https://leanmeancleanapps.github.io/lambda/)

[![Build Pipeline](https://img.shields.io/github/actions/workflow/status/LeanMeanCleanApps/CleanLambdaFunction/build-pipeline.yml?branch=main&label=Build%20Pipeline&logo=github&style=for-the-badge)](https://github.com/LeanMeanCleanApps/CleanLambdaFunction/actions/workflows/build-pipeline.yml)
[![Code Coverage](https://img.shields.io/codecov/c/github/LeanMeanCleanApps/CleanLambdaFunction?label=Code%20Coverage&logo=codecov&logoColor=white&style=for-the-badge)](https://codecov.io/gh/LeanMeanCleanApps/CleanLambdaFunction)
[![NuGet](https://img.shields.io/nuget/v/CleanLambdaFunction.Template?label=NuGet&logo=nuget&logoColor=white&style=for-the-badge)](https://www.nuget.org/packages/CleanLambdaFunction.Template)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CleanLambdaFunction.Template?label=Downloads&logo=nuget&logoColor=white&style=for-the-badge)](https://www.nuget.org/packages/CleanLambdaFunction.Template)
[![License](https://img.shields.io/github/license/LeanMeanCleanApps/CleanLambdaFunction?label=License&style=for-the-badge)](LICENSE.md)

A `dotnet new` template for an [AWS Lambda function](https://docs.aws.amazon.com/lambda/latest/dg/lambda-csharp.html) in .NET 10, built with a streamlined Clean Architecture and shipped as a container image. All the layers you need, none of the ceremony you don't.

It's one of five [Lean Mean Clean Apps](https://leanmeancleanapps.github.io/) templates. They share the same Application and Infrastructure layers and differ only in how they face the world. Same genes, different haircuts.

The guides, the reference and the reasoning behind every opinion live at **[leanmeancleanapps.github.io/lambda](https://leanmeancleanapps.github.io/lambda/)**. This is the version for people who skim. We see you.

## Get started

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). [Docker](https://www.docker.com/products/docker-desktop) is optional, but it's how you run the function locally.

```bash
# Install the template
dotnet new install CleanLambdaFunction.Template

# Create a solution
dotnet new cleanlambdafunction --name Acme.Reviews --authorName "Your Name"
cd Acme.Reviews
```

That's three layers, three test projects, a Dockerfile and a CI pipeline. Go and tell your team you've been working on it all week.

## Run it

The image ships with the Lambda runtime emulator, so it answers exactly like Lambda would. No AWS account needed until you're ready.

```bash
# Build the image and start it with sample data
docker compose up -d --build

# Ask it for a page of movies
curl -X POST http://localhost:9000/2015-03-31/functions/function/invocations \
  -d '{ "query": "global", "limit": 2 }'
```

```json
{
  "Items": [
    {
      "Id": "30fca414-247a-779e-105a-d81aa2506f58",
      "Title": "Assimilated global infrastructure",
      "ReviewCount": 1,
      "AverageStars": 1
    },
    {
      "Id": "c27d6600-79a6-9346-2595-a52b7f07dc3e",
      "Title": "Automated global alliance",
      "ReviewCount": 3,
      "AverageStars": 3.3
    }
  ],
  "TotalCount": 2
}
```

Yes, the sample movies are named by a corporate buzzword generator, and the critics are harsh. The same handler also has `GetAuthors` and `GetReviews`, and [running it locally](https://leanmeancleanapps.github.io/lambda/guides/run-locally/) shows how to call them. When you're ready for the real thing, [deploy it to AWS](https://leanmeancleanapps.github.io/lambda/guides/deploy-to-aws/).

The function is invoked directly, so AWS IAM decides who gets to call it. Want bearer tokens over HTTP? Put it behind an API Gateway HTTP API with a JWT authorizer. Either way, the bouncer stands outside the door, not inside your code.

## What's inside

Three layers, loosely based on Clean Architecture. Fewer layers than an onion, more than a sandwich.

- **Presentation** is the Lambda handler. It sends each request through [Mediator](https://github.com/martinothamar/Mediator) and uses the source-generated Lambda serializer, so cold starts don't pay for reflection.
- **Application** owns the rules. Commands and queries are validated by [FluentValidation](https://github.com/FluentValidation/FluentValidation) and mapped by [Mapperly](https://mapperly.riok.app).
- **Infrastructure** owns the data, with [Entity Framework Core](https://learn.microsoft.com/ef/core) on SQLite. Everything in it is `internal`, so Presentation can wire it up but can't move in.

Tests use [xUnit](https://xunit.net), [Shouldly](https://github.com/shouldly/shouldly) and [NSubstitute](https://github.com/nsubstitute/NSubstitute), and each one gets its own throwaway database. The pipeline builds, tests, demands 95% coverage, scans the container with [Trivy](https://trivy.dev) and runs [CodeQL](https://codeql.github.com), because trust is earned. The [architecture page](https://leanmeancleanapps.github.io/lambda/explanation/architecture/) has the long version, with full credit to [Jason Taylor's template](https://github.com/jasontaylordev/CleanArchitecture) for the inspiration.

## Releases

A new version lands on the 1st of each month, as long as something changed. No changes, no release. We're not here to pad the download stats.

```bash
dotnet new update
```

## The family

| Template | Presentation |
| --- | --- |
| [Clean Minimal API](https://github.com/stphnwlsh/CleanMinimalApi) | ASP.NET Core Minimal APIs |
| [Clean Model Context Protocol](https://github.com/stphnwlsh/CleanModelContextProtocol) | Model Context Protocol server |
| [Clean GraphQL API](https://github.com/stphnwlsh/CleanGraphQLApi) | GraphQL with Hot Chocolate |
| [Clean Web API](https://github.com/stphnwlsh/CleanWebApi) | ASP.NET Core controllers |
| **Clean Lambda Function** | AWS Lambda, you are here |

## Contributing

Bug reports, ideas and pull requests are all welcome, and typo fixes doubly so. Start with the [contributing guide](CONTRIBUTING.md) and the [code of conduct](CODE_OF_CONDUCT.md). Security problems go through the [security policy](SECURITY.md), not an issue.

## Who built this

[<img src="https://github.com/stphnwlsh.png?size=96" width="48" align="left" alt="">](https://github.com/stphnwlsh)
Built by **Stephen Walsh** in spare time, with strong coffee and a healthy disregard for pomp and ceremony. Say hello on [GitHub](https://github.com/stphnwlsh), [LinkedIn](https://www.linkedin.com/in/stphnwlsh), [Medium](https://stphnwlsh.medium.com) or at [stphnwlsh.github.io](https://stphnwlsh.github.io). If this template saves you some time, [sponsoring the work](https://github.com/sponsors/stphnwlsh) keeps all five templates current and the coffee flowing, which is the real dependency here.

## License

[MIT](LICENSE.md). Take it, change it, ship it. Just don't blame us for your cold starts.
