FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY OnlineCatering.sln ./
COPY OnlineCatering/OnlineCatering.csproj OnlineCatering/
RUN dotnet restore OnlineCatering/OnlineCatering.csproj

COPY . ./
RUN dotnet publish OnlineCatering/OnlineCatering.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet OnlineCatering.dll"]
