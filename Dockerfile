FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/release

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime-env
WORKDIR /app

LABEL org.opencontainers.image.authors="fedeantuna"
LABEL org.opencontainers.image.url="https://github.com/fedeantuna/test-identity-server"
LABEL version="1.0.1"
LABEL description="Test Identity Server"

COPY --from=build-env /app/release /app

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "/app/TestIdentityServer.Api.dll"]
