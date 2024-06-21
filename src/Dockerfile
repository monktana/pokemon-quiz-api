FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET 8.0 SDK Docker image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV REDIS_CONNECTIONSTRING=""
WORKDIR /src
COPY ["src/pokemon-quiz-api.csproj", "src/"]
RUN dotnet restore "src/pokemon-quiz-api.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "pokemon-quiz-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "pokemon-quiz-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use the .NET 8.0 runtime Docker image to run the project
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pokemon-quiz-api.dll"]
