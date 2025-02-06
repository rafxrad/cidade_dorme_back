# Imagem base para rodar a aplicação (não o SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagem base para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copiar o arquivo .csproj para o container
COPY ["CidadeDorme/CidadeDorme.csproj", "CidadeDorme/"]
# Restaurar dependências
RUN dotnet restore "CidadeDorme/CidadeDorme.csproj"

# Copiar todo o restante do código
COPY . .
WORKDIR "/src/CidadeDorme"
# Compilar o código
RUN dotnet build "CidadeDorme.csproj" -c Release -o /app/build

# Publicar a aplicação para o diretório /app/publish
FROM build AS publish
RUN dotnet publish "CidadeDorme.csproj" -c Release -o /app/publish

# Usar a imagem base para rodar a aplicação
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CidadeDorme.dll"]
