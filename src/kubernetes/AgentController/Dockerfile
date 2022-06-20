
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AgentController.csproj", "."]
RUN dotnet restore "./AgentController.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AgentController.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgentController.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgentController.dll"]