FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY UrlShortener.Worker/UrlShortener.Worker.csproj UrlShortener.Worker/
COPY UrlShorterner.Application/UrlShortener.Application.csproj UrlShorterner.Application/
COPY UrlShorterner.Domain/UrlShortener.Domain.csproj UrlShorterner.Domain/
COPY UrlShorterner.Infrastructure/UrlShortener.Infrastructure.csproj UrlShorterner.Infrastructure/
COPY UrlShorterner.ServiceDefaults/UrlShortener.ServiceDefaults.csproj UrlShorterner.ServiceDefaults/

RUN dotnet restore UrlShortener.Worker/UrlShortener.Worker.csproj

COPY . .
RUN dotnet publish UrlShortener.Worker/UrlShortener.Worker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.Worker.dll"]