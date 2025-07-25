FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Alertas/Alertas.csproj", "Alertas/"]
RUN dotnet restore "Alertas/Alertas.csproj"
COPY . .
WORKDIR "/src/Alertas"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Alertas.dll"]
