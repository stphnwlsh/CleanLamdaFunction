# Running Clean Lambda Function

The function is packaged as a container image based on the [AWS Lambda .NET base image](https://gallery.ecr.aws/lambda/dotnet), which includes the Lambda runtime interface emulator. That lets you invoke the function locally exactly as Lambda would, without an AWS bill to show for it.

## Database

The function stores its data in a SQLite file, `/tmp/movie-reviews.db`, because `/tmp` is the only place Lambda can write. When a container starts it applies any pending Entity Framework Core migrations. When the `Database__SeedSampleData` environment variable is `true`, it also seeds the same sample data into an empty database. `docker-compose.yml` turns it on, and it's off everywhere else, so a deployed function never starts with fake data. Nobody wants reviews from Bogus in production. Set the `ConnectionStrings__MovieReviews` environment variable to point somewhere else, or to `Data Source=MovieReviews;Mode=Memory;Cache=Shared` to keep everything in memory.

To add a migration after changing the data model:

``` bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <Name> --project ./src/Infrastructure --output-dir Databases/MovieReviews/Migrations
```

## Docker

Build the image and start it.

``` bash
docker build . --target run --tag cleanlambdafunction:local
docker run --rm -p 9000:8080 -e Database__SeedSampleData=true cleanlambdafunction:local
```

Invoke the function from another terminal.

``` bash
curl -X POST "http://localhost:9000/2015-03-31/functions/function/invocations" -d '{"query":"global","offset":0,"limit":5}'
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

The tests never touch `movie-reviews.db`. Each test builds its own in-memory SQLite database from the migrations and the same fixed seed, so no test can change what another one sees and the results are the same every time.
