FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /workspace

COPY global.json Directory.Build.props Waymark.Cache.slnx ./
COPY src/Waymark.Cache/Waymark.Cache.csproj src/Waymark.Cache/
COPY tests/Waymark.Cache.Tests/Waymark.Cache.Tests.csproj tests/Waymark.Cache.Tests/

RUN dotnet restore Waymark.Cache.slnx

COPY . .
RUN dotnet build Waymark.Cache.slnx --no-restore

FROM build AS test
ENTRYPOINT ["dotnet", "test", "Waymark.Cache.slnx", "--no-build"]

