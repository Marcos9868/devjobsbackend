#!/bin/sh

set -e

echo "Waiting for database to be available..."
until dotnet ef database update --project /src/DevJobsBackend.csproj; do
  >&2 echo "Database is unavailable - sleeping"
  sleep 5
done

echo "Starting application..."
exec dotnet DevJobsBackend.dll
