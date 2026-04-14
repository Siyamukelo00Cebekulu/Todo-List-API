# Todo List API

A secure RESTful API for managing personal to-do lists with user authentication, built using ASP.NET Core and C#.

## Features

- **User Authentication**: JWT-based authentication with refresh tokens
- **CRUD Operations**: Full create, read, update, delete operations for to-do items
- **Pagination & Filtering**: Efficient data retrieval with pagination, filtering, and sorting
- **Rate Limiting**: Built-in rate limiting to prevent abuse
- **Data Validation**: Comprehensive input validation and error handling
- **SQLite Database**: Lightweight, file-based database for persistence
- **Unit Tests**: Integration tests covering authentication and to-do management

## Technologies Used

- **ASP.NET Core 9.0**: Web API framework
- **Entity Framework Core**: ORM for database operations
- **SQLite**: Database engine
- **JWT Bearer Authentication**: Token-based authentication
- **xUnit**: Testing framework
- **Swashbuckle/Swagger**: API documentation

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Git](https://git-scm.com/downloads) (optional, for cloning)

## Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd Todo-List-API
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

## Running the API

1. Navigate to the API project directory:
   ```bash
   cd TodoListApi
   ```

2. Run the API:
   ```bash
   dotnet run
   ```

The API will start on `https://localhost:<port-number>` (or `http://localhost:<port-number>` for HTTP).

3. Access the Swagger UI at: `https://localhost:<port-number>/swagger`

## API Endpoints

### Authentication

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "johndoe",
  "password": "SecurePass123!"
}
```

#### Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token-here"
}
```

### Todo Items

All todo endpoints require authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer your-jwt-token-here
```

#### Get Todo Items
```http
GET /api/todo?page=1&pageSize=10&isComplete=false&search=meeting&sortBy=duedate&sortOrder=asc
```

Query parameters:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10, max: 100)
- `isComplete`: Filter by completion status
- `search`: Search in title and description
- `sortBy`: Sort field (createdAt, title, duedate, priority)
- `sortOrder`: Sort order (asc, desc)

#### Get Todo Item by ID
```http
GET /api/todo/{id}
```

#### Create Todo Item
```http
POST /api/todo
Content-Type: application/json

{
  "title": "Complete project report",
  "description": "Finish the quarterly project report by Friday",
  "isComplete": false,
  "dueDate": "2024-12-31T23:59:59Z",
  "priority": 1
}
```

#### Update Todo Item
```http
PUT /api/todo/{id}
Content-Type: application/json

{
  "title": "Updated project report",
  "description": "Finish the quarterly project report by Thursday",
  "isComplete": false,
  "dueDate": "2024-12-30T23:59:59Z",
  "priority": 2
}
```

#### Delete Todo Item
```http
DELETE /api/todo/{id}
```

## Response Examples

### Authentication Response
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresAt": "2024-12-31T15:30:00Z"
}
```

### Todo Item Response
```json
{
  "id": 1,
  "title": "Complete project report",
  "description": "Finish the quarterly project report by Friday",
  "isComplete": false,
  "createdAt": "2024-12-25T10:00:00Z",
  "dueDate": "2024-12-31T23:59:59Z",
  "priority": 1
}
```

### Paginated Todo List Response
```json
[
  {
    "id": 1,
    "title": "Complete project report",
    "description": "Finish the quarterly project report by Friday",
    "isComplete": false,
    "createdAt": "2024-12-25T10:00:00Z",
    "dueDate": "2024-12-31T23:59:59Z",
    "priority": 1
  }
]
```

Headers:
- `X-Total-Count`: Total number of items
- `X-Page`: Current page number
- `X-Page-Size`: Items per page

## Testing

Run the integration tests:
```bash
dotnet test TodoListApi.Tests/TodoListApi.Tests.csproj
```

The tests cover:
- User registration and login
- Token refresh functionality
- Todo CRUD operations
- Authorization checks
- Pagination and sorting

## Database

The API uses SQLite for data persistence. The database file (`todo.db`) is created automatically when the application starts.

### Database Schema

- **Users**: User accounts with authentication data
- **RefreshTokens**: JWT refresh tokens for session management
- **TodoItems**: User's to-do list items

## Security Features

- **Password Hashing**: Uses ASP.NET Core Identity's password hasher
- **JWT Tokens**: Short-lived access tokens (15 minutes) with refresh tokens (7 days)
- **Rate Limiting**: 100 requests per minute per IP address
- **Input Validation**: Comprehensive model validation
- **HTTPS Enforcement**: Redirects HTTP to HTTPS in production
- **SQL Injection Protection**: Parameterized queries via EF Core

## Configuration

Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=todo.db"
  },
  "JwtSettings": {
    "Secret": "your-jwt-secret-key",
    "Issuer": "TodoListApi",
    "Audience": "TodoListApiClient",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  }
}
```

**Important**: Change the JWT secret in production to a secure, random string.

## Error Handling

The API returns appropriate HTTP status codes:
- `200 OK`: Successful operations
- `201 Created`: Resource creation
- `400 Bad Request`: Validation errors
- `401 Unauthorized`: Authentication required or invalid credentials
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `429 Too Many Requests`: Rate limit exceeded
- `500 Internal Server Error`: Server errors

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

This project is licensed under the MIT License.

## URL
https://roadmap.sh/projects/todo-list-api