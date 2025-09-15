# Interview Smartphones Application

A C# .NET 8 Web API with React/TypeScript frontend that manages smartphone data using the DummyJSON API.

## Features

1. **User Authentication**: Login using any user from https://dummyjson.com/users
2. **Smartphone Management**: Display the three most expensive smartphones from the API
3. **Price Updates**: Allow users to increase smartphone prices by a percentage
4. **Comprehensive Logging**: All user inputs and API calls are logged
5. **Error Handling**: Proper error handling for login failures, connection issues, etc.
6. **Unit Tests**: Comprehensive test coverage for services and controllers

## Architecture

### Backend (.NET 8 Web API)
- **Controllers**: Handle HTTP requests and responses
  - `AuthController`: Manages user authentication
  - `SmartphonesController`: Handles smartphone operations
- **Services**: Business logic layer
  - `DummyJsonService`: Wrapper for DummyJSON API calls
- **Models**: Data transfer objects and API response models
- **Logging**: Serilog for structured logging to console and files

### Frontend (React/TypeScript)
- **Components**: 
  - `Login`: User authentication interface
  - `Dashboard`: Main application interface for smartphone management
- **Services**: API client for backend communication
- **Types**: TypeScript interfaces for type safety

## Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- Visual Studio 2022 or VS Code

## Setup Instructions

### Backend Setup

1. Navigate to the main project directory:
   ```bash
   cd InterviewSmartphones
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7174` and `http://localhost:5174`.

### Frontend Setup

1. Navigate to the React app directory:
   ```bash
   cd InterviewSmartphones/ClientApp
   ```

2. Install npm packages:
   ```bash
   npm install
   ```

3. Start the React development server:
   ```bash
   npm start
   ```

The React app will be available at `http://localhost:3000`.

### Running Tests

Run the unit tests:
```bash
dotnet test
```

## Usage

### Sample Login Credentials

You can use any of the DummyJSON users. Here are some sample credentials:

- **Username**: `emilys`, **Password**: `emilyspass`
- **Username**: `atuny0`, **Password**: `9uQFF1Lh`
- **Username**: `hbingley1`, **Password**: `CQutx25i8r`
- **Username**: `rshawe2`, **Password**: `OWsTbMUgFc`
- **Username**: `yraigatt3`, **Password**: `sRQxjPfdS`

### Application Flow

1. **Login**: Enter valid DummyJSON credentials
2. **View Smartphones**: The app automatically fetches and displays the top 3 most expensive smartphones
3. **Update Prices**: Enter a percentage to increase all smartphone prices
4. **View Updated Prices**: See the new prices with original prices shown for comparison

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `GET /api/auth/users` - Get users list (for demo purposes)

### Smartphones
- `GET /api/smartphones/top-three` - Get top 3 most expensive smartphones (requires Bearer token)
- `PUT /api/smartphones/update-prices` - Update smartphone prices (requires Bearer token)

## Project Structure

```
InterviewSmartphones/
??? Controllers/
?   ??? AuthController.cs
?   ??? SmartphonesController.cs
??? Services/
?   ??? IDummyJsonService.cs
?   ??? DummyJsonService.cs
??? Models/
?   ??? User.cs
?   ??? Product.cs
?   ??? ApiResponse.cs
??? ClientApp/
?   ??? public/
?   ??? src/
?   ?   ??? components/
?   ?   ?   ??? Login.tsx
?   ?   ?   ??? Login.css
?   ?   ?   ??? Dashboard.tsx
?   ?   ?   ??? Dashboard.css
?   ?   ??? services/
?   ?   ?   ??? api.ts
?   ?   ??? types/
?   ?   ?   ??? index.ts
?   ?   ??? App.tsx
?   ?   ??? App.css
?   ?   ??? index.tsx
?   ??? package.json
?   ??? tsconfig.json
??? Program.cs
```

## Technical Decisions

### Backend Architecture
- **Service Layer Pattern**: Separated business logic into services for better testability
- **Dependency Injection**: Used built-in DI container for loose coupling
- **API Response Wrapper**: Consistent response format across all endpoints
- **Comprehensive Logging**: Serilog for structured logging with file and console outputs

### Frontend Architecture
- **Component-Based**: React functional components with hooks
- **TypeScript**: Strong typing for better development experience
- **Axios**: HTTP client with interceptors for authentication and error handling
- **Local Storage**: Token persistence for user sessions
- **CSS Modules**: Scoped styling for components

### Error Handling
- Network timeouts and connection errors
- Invalid authentication tokens
- API rate limiting
- Invalid user input validation
- Graceful degradation for UI components

### Security Considerations
- Bearer token authentication
- Input validation on both client and server
- CORS configuration for frontend integration
- No sensitive data in local storage (only tokens)

## Logging

The application logs:
- User login attempts (success/failure)
- API calls to DummyJSON endpoints
- Request/response data (excluding sensitive information)
- Error conditions and exceptions
- Performance metrics

Logs are written to:
- Console (for development)
- Log files in the `logs/` directory (rolling daily files)

## Testing

The test suite includes:
- **Service Tests**: HTTP client mocking, error handling, data transformation
- **Controller Tests**: Request validation, authentication, response formatting
- **Unit Tests**: Individual method testing with mocking
- **Integration Testing**: End-to-end API testing scenarios

Run tests with coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment Notes

For production deployment:
1. Build React app: `npm run build` in ClientApp directory
2. Publish .NET app: `dotnet publish -c Release`
3. Configure HTTPS certificates
4. Set production connection strings and API URLs
5. Configure logging levels for production

## Future Enhancements

- Add user registration functionality
- Implement product categories and filtering
- Add shopping cart functionality
- Implement real-time price updates with SignalR
- Add product image gallery
- Implement user preferences and favorites
- Add caching layer for improved performance
- Implement API rate limiting and throttling