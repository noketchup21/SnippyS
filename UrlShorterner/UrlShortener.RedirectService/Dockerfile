FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY UrlShortener.RedirectService/UrlShortener.RedirectService.csproj UrlShortener.RedirectService/
COPY UrlShorterner.Application/UrlShortener.Application.csproj UrlShorterner.Application/
COPY UrlShorterner.Domain/UrlShortener.Domain.csproj UrlShorterner.Domain/
COPY UrlShorterner.Infrastructure/UrlShortener.Infrastructure.csproj UrlShorterner.Infrastructure/
COPY UrlShorterner.ServiceDefaults/UrlShortener.ServiceDefaults.csproj UrlShorterner.ServiceDefaults/

RUN dotnet restore UrlShortener.RedirectService/UrlShortener.RedirectService.csproj

COPY . .
RUN dotnet publish UrlShortener.RedirectService/UrlShortener.RedirectService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "UrlShortener.RedirectService.dll"]