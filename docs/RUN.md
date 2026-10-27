# Running Clean Lambda Function

The function is packaged as a container image based on the [AWS Lambda .NET base image](https://gallery.ecr.aws/lambda/dotnet), which includes the Lambda runtime interface emulator. That lets you invoke the function locally exactly as Lambda would.

## Docker

Build the image and start it.

``` bash
docker build . --target run --tag cleanlamdafunction:local
docker run --rm -p 9000:8080 cleanlamdafunction:local
```

Invoke the function from another terminal.

``` bash
curl -X POST "http://localhost:9000/2015-03-31/functions/function/invocations" -d '""'
```

## Docker Compose

`docker-compose.yml` does the same build and run in one step.

``` bash
docker compose up -d --build
```

To shut it down:

``` bash
docker compose down
```

## Tests

``` bash
dotnet test
```

## Deploying to AWS

`src/Presentation/aws-lambda-tools-defaults.json` holds the defaults for the [Amazon.Lambda.Tools](https://github.com/aws/aws-extensions-for-dotnet-cli) CLI. Fill in your profile and region, then deploy the container image.

``` bash
dotnet tool install -g Amazon.Lambda.Tools
dotnet lambda deploy-function --project-location ./src/Presentation
```
