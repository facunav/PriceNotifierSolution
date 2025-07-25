FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Producer.Api/Producer.Api.csproj", "Producer.Api/"]
RUN dotnet restore "Producer.Api/Producer.Api.csproj"
COPY . .
WORKDIR "/src/Producer.Api"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Producer.Api.dll"]
