# Imagem base do ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Imagem do SDK do .NET para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["chatApi.csproj", "./"]
RUN dotnet restore "./chatApi.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "chatApi.csproj" -c Release -o /app/build

# Publicação da aplicação
FROM build AS publish
RUN dotnet publish "chatApi.csproj" -c Release -o /app/publish

# Container final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "chatApi.dll"]
