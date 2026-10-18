FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HomeHub/HomeHub.csproj HomeHub/
RUN dotnet restore HomeHub/HomeHub.csproj

COPY HomeHub/ HomeHub/
RUN dotnet publish HomeHub/HomeHub.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet HomeHub.dll"]
