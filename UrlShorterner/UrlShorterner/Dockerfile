FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY UrlShorterner/UrlShortener.csproj UrlShorterner/
COPY UrlShorterner.Application/UrlShortener.Application.csproj UrlShorterner.Application/
COPY UrlShorterner.Domain/UrlShortener.Domain.csproj UrlShorterner.Domain/
COPY UrlShorterner.Infrastructure/UrlShortener.Infrastructure.csproj UrlShorterner.Infrastructure/
COPY UrlShortener.Contracts/UrlShortener.Contracts.csproj UrlShortener.Contracts/
COPY UrlShorterner.ServiceDefaults/UrlShortener.ServiceDefaults.csproj UrlShorterner.ServiceDefaults/

RUN dotnet restore UrlShorterner/UrlShortener.csproj

COPY . .
RUN dotnet publish UrlShorterner/UrlShortener.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "UrlShortener.dll"]