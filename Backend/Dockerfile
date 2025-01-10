# Use a lightweight .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file first
COPY src/Sessions.sln ./

# Copy all project files (to benefit from layer caching during restore)
COPY src/Sessions/Sessions.csproj ./Sessions/
COPY src/DDD/DDD.csproj ./DDD/

# Restore dependencies
RUN dotnet restore Sessions.sln

# Copy the rest of the application
COPY src/ ./

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Debugging: List files in the /app directory
RUN ls -la /app

# Run the application
ENTRYPOINT ["dotnet", "Sessions.dll"]
