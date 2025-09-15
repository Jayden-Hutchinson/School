# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Set Working Directory as /src
WORKDIR /src
# Copy everything from current project dir to working dir
COPY . .
# dotnet command: restore all proj dependencies
RUN dotnet restore "./CytoNET.csproj" --disable-parallel
# dotnet command: publish artifacts to output directory, /app
RUN dotnet publish "./CytoNET.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
# Copy all the artifacts that ended up in the output dir from the build stage into current working dir
COPY --from=build /app ./

ENTRYPOINT [ "dotnet",  "CytoNET.dll"]