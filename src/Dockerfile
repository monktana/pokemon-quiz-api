FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET 8.0 SDK Docker image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV REDIS_CONNECTIONSTRING=""

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/build

# Use the .NET 8.0 runtime Docker image to run the project
FROM base AS final
WORKDIR /app
COPY --from=publish /app/build .
ENTRYPOINT ["dotnet", "pokemon-quiz-api.dll"]
