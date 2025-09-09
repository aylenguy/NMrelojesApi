# -----------------------
# Etapa de build
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos primero los csproj de cada proyecto
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Web/Web.csproj src/Web/

# Restauramos dependencias
RUN dotnet restore src/Web/Web.csproj

# Copiamos el resto del código
COPY . .

# Publicamos la aplicación en modo Release
RUN dotnet publish src/Web/Web.csproj -c Release -o /app/publish

# -----------------------
# Etapa runtime
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Railway define la variable $PORT automáticamente
ENTRYPOINT ["sh", "-c", "dotnet Web.dll --urls http://*:$PORT"]
