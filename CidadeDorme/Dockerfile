# Etapa de build: Usando o SDK do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR .

# Copiar o arquivo .csproj e restaurar dependências
COPY ["CidadeDorme/CidadeDorme.csproj", "CidadeDorme/"]

# Restaurar as dependências
RUN dotnet restore "CidadeDorme/CidadeDorme.csproj"

# Copiar o resto do código e compilar
COPY ["CidadeDorme/", "CidadeDorme/"]
WORKDIR CidadeDorme
RUN dotnet build "CidadeDorme.csproj" -c Release -o /app/build

# Etapa de publicação
FROM build as publish
RUN dotnet publish "CidadeDorme.csproj" -c Release -o /app/publish

# Etapa final: Imagem para execução (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

EXPOSE 8080
WORKDIR /app
# Copiar os arquivos compilados do contêiner de build
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CidadeDorme.dll"]

ENV ASPNETCORE_ENVIRONMENT=Development