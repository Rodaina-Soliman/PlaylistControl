# Playlist Control API

A RESTful API for managing users, playlists, and songs. Built with ASP.NET Core Web API and Entity Framework Core.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)

## Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express / LocalDB)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads) (optional, for cloning)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Rodaina-Soliman/PlaylistControl.git
cd PlaylistControl
```

### 2. Install Required Packages

The project uses the following NuGet packages (already defined in `PlaylistControl.csproj`):

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.OpenApi
```

Or restore all packages using:

```bash
dotnet restore
```

### 3. Database Setup

#### Option A: Using SQL Server LocalDB (Recommended for Development)

1. **Create the Database**:
```bash
dotnet ef database update
```

2. **If Migrations don't exist**, create them:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Option B: Using SQL Server Express or Full SQL Server

1. **Update the connection string** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PlaylistControlDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Replace `YOUR_SERVER_NAME` with:
- `(localdb)\MSSQLLocalDB` for LocalDB
- `localhost` or `SQLEXPRESS` for SQL Server Express
- Your server name for full SQL Server

#### Option C: Using SQL Server with Authentication

For SQL Server with username/password:
```json
"DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PlaylistControlDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

### 4. Configuration

#### `appsettings.json` (Development)

The default configuration file should look like:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PlaylistControlDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AllowedHosts": "*"
}
```

#### `appsettings.Development.json`

For development-specific settings, create this file:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PlaylistControlDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Running the Application

### Using Command Line
```bash
dotnet run

```

## API Endpoints

### User Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/user` | Get all users |
| GET | `/user/{userId}` | Get user by ID |
| GET | `/user/{userId}/playlists` | Get all playlists for a user |
| POST | `/user` | Add a new user |
| POST | `/user/{userId}/playlists` | Create a new playlist for a user |
| POST | `/user/{userId}/playlists/{playlistId}` | Add an existing playlist to a user |
| PUT | `/user/{userId}` | Update user information |
| PUT | `/user/{userId}/playlists/{playlistId}` | Update a playlist (must belong to user) |
| DELETE | `/user/{userId}` | Delete a user |
| DELETE | `/user/{userId}/playlists/{playlistId}` | Remove a playlist from a user |
| PUT | `/user/{userId}/playlists/{playlistId}` | Update playlist information |
| POST | `/user/{userId}/songs/{songId}/addToPlaylist/{playlistId}` | Add a song to a playlist |
| DELETE | `/user/{userId}/playlists/{playlistId}/RemoveFromPlaylist/{songId}` | Remove a song from a playlist |

### Playlist Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/playlist` | Get all playlists |
| GET | `/playlist/{playlistId}` | Get playlist by ID |
| GET | `/playlist/{playlistId}/songs` | Get all songs in a playlist |

### Song Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/song` | Get all songs |
| GET | `/song/{songId}` | Get song by ID |
| GET | `/song/{songId}/details` | Get detailed song information |
| POST | `/song` | Add a new song |
| PUT | `/song/{songId}` | Update song information |
| DELETE | `/song/{songId}` | Delete a song |

## Testing the API

### With PlaylistControl.http

1. **Add C# Dev Tool Extension in VS Code**
2. **Run the API**
   - Using `dotnet run` 
3. **Write Requests**
   - There are examples in the file
4. **Click the `Send Request` Option**
   - It appears above each request

## Troubleshooting

### Common Issues

1. **Database Connection Error:**
   - Ensure SQL Server is running
   - Verify connection string in `appsettings.json`
   - Check if the database exists: `dotnet ef database update`

2. **Migration Issues:**
   - Remove existing migrations: `dotnet ef migrations remove`
   - Add new migration: `dotnet ef migrations add InitialCreate`
   - Update database: `dotnet ef database update`

3. **Package Restore Failures:**
   - Clear NuGet cache: `dotnet nuget locals all --clear`
   - Restore packages: `dotnet restore`

## Project Structure

```
PlaylistControl/
├── Controllers/
│   ├── PlaylistController.cs
│   ├── SongController.cs
│   └── UserController.cs
├── Models/
│   ├── Playlist.cs
│   ├── Song.cs
│   ├── SongPlaylist.cs
│   ├── User.cs
│   └── UserPlaylist.cs
├── Services/
│   ├── PlaylistService.cs
│   ├── SongService.cs
│   └── UserService.cs
├── Data/
│   └── PlaylistControlDbContext.cs
├── Migrations/
├── appsettings.Development.json
├── appsettings.json
├── Program.cs
├── PlaylistControl.http
├── PlaylistControl.csproj
└── README.md
```

## Technologies Used

- **.NET 10.0** - Framework
- **ASP.NET Core Web API** - Web framework
- **Entity Framework Core 10.0** - ORM
- **SQL Server** - Database
