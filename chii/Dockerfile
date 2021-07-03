# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /code

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore chii.csproj

# copy everything else and build app
RUN dotnet publish -c debug -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "chii.dll"]
