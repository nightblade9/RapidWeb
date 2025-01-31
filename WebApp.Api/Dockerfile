# Use .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# Copy solution and project files to restore dependencies
COPY WebApp.sln .  
COPY WebApp.Api/WebApp.Api.csproj WebApp.Api/

# Restore dependencies
RUN dotnet restore WebApp.Api/WebApp.Api.csproj

# Copy the entire source code
COPY . .  

# Publish the app (build and optimize for release)
WORKDIR /src/WebApp.Api
RUN dotnet publish -c Release -o /app/publish

# Use .NET runtime image for the final app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published app from the build-env stage
COPY --from=build-env /app/publish .

# Expose the API ports
EXPOSE 7145

# Entry point for the application
ENTRYPOINT ["dotnet", "WebApp.Api.dll"]
