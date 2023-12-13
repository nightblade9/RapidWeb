# Build the code in one layer
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /Stocks.Web

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish Stocks.Web -c Release -o out

# Build runtime image using binaries from our build-env layer
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /Stocks.Web
COPY --from=build-env /Stocks.Web/out .
ENTRYPOINT ["dotnet", "Stocks.Web.dll"]