#!/bin/sh

echo "Waiting for PostgreSQL..."
until pg_isready -h postgres -p 5432 -U presidio; do
  echo "Postgres is unavailable - sleeping"
  sleep 2
done

echo "Postgres is ready. Running EF Core migrations..."


cd /app

dotnet ef database update --project ElearnCourseAPI.csproj

echo "Starting application..."
exec dotnet ElearnCourseAPI.dll
