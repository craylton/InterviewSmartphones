# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Node.js for React build
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs

# Copy project files
COPY ["InterviewSmartphones/InterviewSmartphones.csproj", "InterviewSmartphones/"]
COPY ["InterviewSmartphones.Tests/InterviewSmartphones.Tests.csproj", "InterviewSmartphones.Tests/"]

# Restore dependencies
RUN dotnet restore "InterviewSmartphones/InterviewSmartphones.csproj"

# Copy all source code
COPY . .

# Build React app
WORKDIR /src/InterviewSmartphones/ClientApp
RUN npm install
RUN npm run build

# Build .NET app
WORKDIR /src/InterviewSmartphones
RUN dotnet build "InterviewSmartphones.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "InterviewSmartphones.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InterviewSmartphones.dll"]