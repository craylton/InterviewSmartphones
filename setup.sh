#!/bin/bash

echo "?? Starting Interview Smartphones Application Setup"

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "? .NET 8 SDK is required but not installed."
    echo "Please install .NET 8 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "? Node.js is required but not installed."
    echo "Please install Node.js from: https://nodejs.org/"
    exit 1
fi

echo "? Prerequisites check passed"

# Restore .NET packages
echo "?? Restoring .NET packages..."
dotnet restore

# Build .NET project
echo "?? Building .NET project..."
dotnet build

# Install React dependencies
echo "?? Installing React dependencies..."
cd InterviewSmartphones/ClientApp
npm install

echo "? Setup completed successfully!"
echo ""
echo "?? To start the application:"
echo "1. Backend: Run 'dotnet run' in the InterviewSmartphones directory"
echo "2. Frontend: Run 'npm start' in the InterviewSmartphones/ClientApp directory"
echo ""
echo "?? The application will be available at:"
echo "- Backend API: https://localhost:7174"
echo "- Frontend: http://localhost:3000"
echo ""
echo "?? To run tests:"
echo "- Backend tests: 'dotnet test'"
echo "- Frontend tests: 'npm test' (in ClientApp directory)"