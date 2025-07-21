# ----- STAGE 1: Build -----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# Copy the project file and restore dependencies
COPY ./Day55_July_21st.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY . ./
RUN dotnet publish -c Release -o /out

# ----- STAGE 2: Runtime -----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "Day55_July_21st.dll"]
