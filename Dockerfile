FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DevJobsBackend.csproj", "."]
RUN dotnet restore "./DevJobsBackend.csproj"
COPY . .
WORKDIR "/src/DevJobsBackend"
RUN dotnet build "../DevJobsBackend.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "../DevJobsBackend.csproj" -c Release -o /app/publish

# Migrations
FROM publish AS migrations
WORKDIR /app/publish
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef database update

# End
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "DevJobsBackend.dll"]
