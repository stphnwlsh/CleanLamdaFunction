# Clean Lambda Function

A `dotnet new` template for an [AWS Lambda function](https://docs.aws.amazon.com/lambda/latest/dg/lambda-csharp.html) built with a streamlined version of Clean Architecture, packaged as a container image. All the layers you need, none of the ceremony you don't.

It's one of five [Lean Mean Clean Apps](https://leanmeancleanapps.github.io/) templates, for Minimal APIs, Web API controllers, GraphQL, Model Context Protocol and AWS Lambda, that share the same Application and Infrastructure layers, tests and CI pipeline. Full docs live at [leanmeancleanapps.github.io/lambda](https://leanmeancleanapps.github.io/lambda/).

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop) (optional, to build and run the container image)

## Install

``` bash
dotnet new install CleanLambdaFunction.Template
```

## Create a solution

``` bash
dotnet new cleanlambdafunction --name {YOUR_SOLUTION_NAMESPACE} --authorName "{YOUR_NAME}"
```

| Option | Description |
| --- | --- |
| `--name` | The name of the solution, also used as the root namespace. |
| `--authorName` | The name of the author of the solution. |

The function is invoked directly, so AWS IAM decides who can call it. To call it over HTTP with bearer tokens, put it behind an API Gateway HTTP API with a JWT authorizer.

## Run it

From the new solution's folder, build the image and start it. The image includes the Lambda runtime interface emulator, so you can invoke the function locally exactly as Lambda would. No AWS account needed until you're ready.

``` bash
docker compose up -d --build
curl -X POST "http://localhost:9000/2015-03-31/functions/function/invocations" -d '{"query":"global","offset":0,"limit":5}'
```

To deploy it, fill in your profile and region in `src/Presentation/aws-lambda-tools-defaults.json`, then run `dotnet lambda deploy-function --project-location ./src/Presentation` with [Amazon.Lambda.Tools](https://github.com/aws/aws-extensions-for-dotnet-cli).

Run the tests with `dotnet test`. Every way to run the solution is in `docs/RUN.md` inside it.

## What's inside

- **Presentation**: AWS Lambda handlers packaged as a container image, using the source-generated Lambda serializer
- **Application**: commands and queries through [Mediator](https://github.com/martinothamar/Mediator), validation with [FluentValidation](https://github.com/FluentValidation/FluentValidation) and mapping with [Mapperly](https://mapperly.riok.app)
- **Infrastructure**: [Entity Framework Core](https://learn.microsoft.com/ef/core) on SQLite with migrations and optional sample data
- **Tests**: [xUnit](https://xunit.net), [Shouldly](https://github.com/shouldly/shouldly) and [NSubstitute](https://github.com/nsubstitute/NSubstitute), each test on its own in-memory database
- A Dockerfile that runs as a non-root user, a `docker-compose.yml`, and a GitHub Actions pipeline with tests, a coverage gate, a container scan and CodeQL

## Updates

A new version is released on the 1st of each month with every change merged since the previous release, as long as something has changed. No change, no new version number. We're not here to pad the download stats. To update an installed template:

``` bash
dotnet new update
```

What changed in each version is in the [release notes](https://github.com/stphnwlsh/CleanLamdaFunction/releases). To remove the template, run `dotnet new uninstall CleanLambdaFunction.Template`.

## More help

- [Docs site](https://leanmeancleanapps.github.io/lambda/) for guides, a function reference and why it's built this way
- [README on GitHub](https://github.com/stphnwlsh/CleanLamdaFunction#readme) for the architecture, every feature and the Docker build options
- [Running guide](https://github.com/stphnwlsh/CleanLamdaFunction/blob/main/docs/RUN.md) for every way to run, configure and test the solution
- [GitHub issues](https://github.com/stphnwlsh/CleanLamdaFunction/issues) for bugs and ideas
- [Security policy](https://github.com/stphnwlsh/CleanLamdaFunction/security/policy) to report a vulnerability privately

## Support

If this template saves you time, please consider [sponsoring me on GitHub](https://github.com/sponsors/stphnwlsh). It keeps all five templates current, and the coffee flowing.
