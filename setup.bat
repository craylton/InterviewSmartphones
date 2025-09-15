@echo off
echo ?? Starting Interview Smartphones Application Setup

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ? .NET 8 SDK is required but not installed.
    echo Please install .NET 8 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0
    exit /b 1
)

REM Check if Node.js is installed
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ? Node.js is required but not installed.
    echo Please install Node.js from: https://nodejs.org/
    exit /b 1
)

echo ? Prerequisites check passed

REM Restore .NET packages
echo ?? Restoring .NET packages...
dotnet restore

REM Build .NET project
echo ?? Building .NET project...
dotnet build

REM Install React dependencies
echo ?? Installing React dependencies...
cd InterviewSmartphones\ClientApp
npm install

echo ? Setup completed successfully!
echo.
echo ?? To start the application:
echo 1. Backend: Run 'dotnet run' in the InterviewSmartphones directory
echo 2. Frontend: Run 'npm start' in the InterviewSmartphones\ClientApp directory
echo.
echo ?? The application will be available at:
echo - Backend API: https://localhost:7174
echo - Frontend: http://localhost:3000
echo.
echo ?? To run tests:
echo - Backend tests: 'dotnet test'
echo - Frontend tests: 'npm test' (in ClientApp directory)
pause