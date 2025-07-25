FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Consumer.Worker/Consumer.Worker.csproj", "Consumer.Worker/"]
RUN dotnet restore "Consumer.Worker/Consumer.Worker.csproj"
COPY . .
WORKDIR "/src/Consumer.Worker"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Consumer.Worker.dll"]


