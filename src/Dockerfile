# Use the .NET 8.0 SDK Docker image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o build

# Use the .NET 8.0 runtime Docker image to run the project
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV Sentry:Debug=false
ENV Sentry:TracesSampleRate=0.5
ENV Sentry:ProfilesSampleRate=1
RUN apt-get update && apt-get install -y curl
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "pokemon-quiz-api.dll"]

#HEALTHCHECK --start-period=30s \
# CMD curl -f http://localhost:8080/api/v1/health || exit 1