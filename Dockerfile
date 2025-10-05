FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TmdbAppApi.csproj", "./"]
RUN dotnet restore "TmdbAppApi.csproj"
COPY . .
RUN dotnet build "TmdbAppApi.csproj" -c Release /p:UseAppHost=false
RUN dotnet publish "TmdbAppApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TmdbAppApi.dll"]
