# Use the .NET 8.0 SDK Docker image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV ConnectionStrings__Redis=""
WORKDIR /app
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o build

# Use the .NET 8.0 runtime Docker image to run the project
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "pokemon-quiz-api.dll"]
