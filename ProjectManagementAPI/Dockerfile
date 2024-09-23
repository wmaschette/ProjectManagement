# Etapa base para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar o arquivo .csproj para o diretório correto
COPY ["ProjectManagementAPI/ProjectManagementAPI.csproj", "ProjectManagementAPI/"]

# Restaurar dependências
RUN dotnet restore "ProjectManagementAPI/ProjectManagementAPI.csproj"

# Copiar todo o conteúdo do projeto
COPY . .

# Definir o diretório de trabalho e compilar o projeto
WORKDIR "/src/ProjectManagementAPI"
RUN dotnet build "ProjectManagementAPI.csproj" -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish "ProjectManagementAPI.csproj" -c Release -o /app/publish

# Etapa final para rodar a aplicação
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ProjectManagementAPI.dll"]