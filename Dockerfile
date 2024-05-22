FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./DevJobsBackend.csproj"
COPY . .
WORKDIR "/src/DevJobsBackend"
RUN dotnet build "../DevJobsBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "../DevJobsBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevJobsBackend.dll"]
