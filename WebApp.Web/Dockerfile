# Use .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# Copy solution and project files to restore dependencies
COPY WebApp.sln .  
COPY WebApp.Web/WebApp.Web.csproj WebApp.Web/

# Restore dependencies
RUN dotnet restore WebApp.Web/WebApp.Web.csproj

# Copy the entire source code
COPY . .  

# Publish the app (build and optimize for release)
WORKDIR /src/WebApp.Web
RUN dotnet publish -c Release -o /app/publish

# Use .NET runtime image for the final app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published app from the build-env stage
COPY --from=build-env /app/publish .

# Expose the ports (adjust based on your actual configuration)
EXPOSE 5000
EXPOSE 5001

# Entry point for the application
ENTRYPOINT ["dotnet", "WebApp.Web.dll"]
