# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the csproj and restore as distinct layers
COPY *.sln .
COPY SearchEngine.API/*.csproj ./SearchEngine.API/
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . .
WORKDIR /app/SearchEngine.API
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image as a runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/SearchEngine.API/out ./

# Expose the port the app runs on
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "SearchEngine.API.dll"]