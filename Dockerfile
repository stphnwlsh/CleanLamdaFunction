# Setup Build Image
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine@sha256:cc02b814237a76c4735fb6eeee87181597b5e56f2ce458a435e527674d91f4d7 AS build

# Build, Test and Publish ARGS
ARG VERSION_PREFIX=1.0.0.0
ARG VERSION_SUFFIX
ARG ENVIRONMENT=docker

# Build, Test and Publish ENVS
ENV DOTNET_ENVIRONMENT=${ENVIRONMENT}

WORKDIR /sln

# Dotnet Restore
COPY ./*.sln ./NuGet.config ./Directory.Build.props ./Directory.Packages.props ./
COPY --parents src/*/*.csproj src/*/packages.lock.json tests/*/*.csproj tests/*/packages.lock.json ./

RUN dotnet restore --locked-mode -v minimal

# Copy Remaining Files
COPY . .

# Dotnet Build
RUN dotnet build --no-restore -c Release -v minimal -p:VersionPrefix=${VERSION_PREFIX} -p:VersionSuffix=${VERSION_SUFFIX}

# Dotnet Test
FROM build AS test
RUN dotnet test --no-restore --no-build -c Release -v minimal -p:CollectCoverage=true  -p:CoverletOutput=../results/ -p:MergeWith="../results/coverage.json" -p:CoverletOutputFormat=opencover%2cjson -m:1 -p:ExcludeByFile="**/Migrations/*.cs%2c**/obj/**/*.cs" -p:ExcludeByAttribute="GeneratedCodeAttribute"

FROM scratch AS coverage
COPY --from=test /sln/tests/results/*.xml .

# Dotnet Publish
FROM build AS publish
ARG VERSION_PREFIX
ARG VERSION_SUFFIX
RUN dotnet publish ./src/**/Presentation.csproj --no-build -c Release -v quiet -o app -p:VersionPrefix=${VERSION_PREFIX} -p:VersionSuffix=${VERSION_SUFFIX} --no-self-contained

# Runtime Image
FROM public.ecr.aws/lambda/dotnet:10@sha256:3fe5525cd0bc056b37ce8d22158c170688af25ff149490e2310299972a889ed4 AS run

# Apply OS security fixes published since the base image was built. Amazon Linux
# 2023 pins packages to the release the image was built from, so ask for the latest.
RUN dnf upgrade -y --releasever=latest && dnf clean all

WORKDIR /var/task
COPY --from=publish /sln/app .
CMD [ "Presentation::CleanLambdaFunction.Presentation.LambdaFunction.Handler::GetMovies" ]
