# Buildersoft LLC
# Buildersoft Vortex Project
# -----------------------------------------------------------
# Buildersoft Vortex v4.0.0-alpha{x} Docker File


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Install needed librarise to run rocksdb embedded
RUN apt-get update && apt-get install -y libsnappy-dev zlib1g-dev libbz2-dev libyaml-dev libltdl-dev libtool


WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Vortex.Server/Vortex.Server.csproj", "src/Vortex.Server/"]
COPY ["src/Vortex.Core/Vortex.Core.csproj", "src/Vortex.Core/"]
COPY ["src/Vortex.Infrastructure/Vortex.Infrastructure.csproj", "src/Vortex.Infrastructure/"]
RUN dotnet restore "src/Vortex.Server/Vortex.Server.csproj"
COPY . .
WORKDIR "/src/src/Vortex.Server"
RUN dotnet build "Vortex.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vortex.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vortex.Server.dll"]