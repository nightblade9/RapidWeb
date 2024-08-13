# Build the code in one layer
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /WebApp.Web

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish WebApp.Web -c Release -o out

# Build runtime image using binaries from our build-env layer
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /WebApp.Web
COPY --from=build-env /WebApp.Web/out .
ENTRYPOINT ["dotnet", "WebApp.Web.dll"]