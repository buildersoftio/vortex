# Buildersoft LLC
# Buildersoft Cerebro Project
# -----------------------------------------------------------
# Buildersoft Cerebro v4.0.0-alpha{x} Docker File


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

# Install needed librarise to run rocksdb embedded
RUN apt-get update && apt-get install -y libsnappy-dev zlib1g-dev libbz2-dev libyaml-dev libltdl-dev libtool


WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Cerebro.Server/Cerebro.Server.csproj", "src/Cerebro.Server/"]
COPY ["src/Cerebro.Core/Cerebro.Core.csproj", "src/Cerebro.Core/"]
COPY ["src/Cerebro.Infrastructure/Cerebro.Infrastructure.csproj", "src/Cerebro.Infrastructure/"]
RUN dotnet restore "src/Cerebro.Server/Cerebro.Server.csproj"
COPY . .
WORKDIR "/src/src/Cerebro.Server"
RUN dotnet build "Cerebro.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cerebro.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cerebro.Server.dll"]