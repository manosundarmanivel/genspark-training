# Step 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /src

# Install EF Core CLI
RUN dotnet tool install --global dotnet-ef

# Make EF Core CLI available in PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy everything
COPY . ./

# Restore dependencies
RUN dotnet restore

# Run EF Core migrations (ensure your project has a valid DbContext and connection string)
RUN dotnet ef database update

# Publish the app
RUN dotnet publish -c Release -o /app/publish

# Step 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Set working directory
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Create required folders
RUN mkdir -p /app/UploadedThumbnails /app/Uploads

# Set environment (optional)
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "ElearnCourseAPI.dll"]
