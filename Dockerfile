# Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DevJobsBackend.csproj", "./"]
RUN dotnet restore "./DevJobsBackend.csproj"
COPY . .
WORKDIR "/src/DevJobsBackend"
RUN dotnet build "../DevJobsBackend.csproj" -c Release -o /app/build
RUN dotnet publish "../DevJobsBackend.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src /src
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENV ASPNETCORE_URLS=http://+:5000
COPY entrypoint.sh .
RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]
