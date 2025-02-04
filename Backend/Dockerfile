# Use a lightweight .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Install Chromium and required dependencies for PuppeteerSharp
RUN apt-get update && apt-get install -y --no-install-recommends \
    chromium \
    libnss3 libatk1.0-0 libatk-bridge2.0-0 libcups2 \
    libxcomposite1 libxrandr2 libxdamage1 libgbm1 libpango-1.0-0 \
    libpangocairo-1.0-0 libasound2 libxshmfence1 libx11-xcb1 \
    libxcb-dri3-0 libglib2.0-0 \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Set Chromium path as an environment variable
ENV PUPPETEER_EXECUTABLE_PATH="/usr/bin/chromium"

# Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file first
COPY src/Sessions.sln ./

# Copy all project files (to benefit from layer caching during restore)
COPY src/Sessions/Sessions.csproj ./Sessions/
COPY src/OpenDDD/OpenDDD.csproj ./OpenDDD/

# Restore dependencies
RUN dotnet restore Sessions.sln

# Copy the rest of the application
COPY src/ ./

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app

# Ensure the Data directory exists
RUN mkdir -p /app/Data/EfCore && chmod -R 777 /app/Data/EfCore

COPY --from=build /app/publish .

# Debugging: List files in the /app directory
RUN ls -la /app

# Run the application
ENTRYPOINT ["dotnet", "Sessions.dll"]
