# --- Etapa de build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ApiEcommerce.csproj .
RUN dotnet restore ApiEcommerce.csproj

COPY . .
RUN dotnet publish ApiEcommerce.csproj -c Release -o /app/publish --no-restore

# --- Etapa de runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
# Render inyecta la variable PORT en tiempo de ejecución; la app debe escuchar ahí.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} dotnet ApiEcommerce.dll"]
