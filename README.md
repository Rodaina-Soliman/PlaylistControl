# PlaylistControl

## Overview

A RESTful API for managing users, playlists, and songs. Built with ASP.NET Core Web API and Entity Framework Core.

---

## Project Structure

### Solution-Level Structure

```
PlaylistControl.slnx
├── src/
│   ├── PlaylistControl.Domain/
│   ├── PlaylistControl.Application/
│   ├── PlaylistControl.Infrastructure/
│   └── PlaylistControl.Api/
└── tests/
    ├── PlaylistControl.UnitTests/
    └── PlaylistControl.IntegrationTests/
```

---

### Domain Layer — `PlaylistControl.Domain`

```
PlaylistControl.Domain/
├── Entities/
│   ├── User.cs
│   ├── Song.cs
│   ├── Playlist.cs
│   └── UserPlaylist.cs
└── Exceptions/
    ├── DomainException.cs
    ├── PlaylistNotFoundException.cs
    ├── UserNotFoundException.cs
    ├── SongNotFoundException.cs
    ├── NotPlaylistOwnerException.cs
    └── PrivatePlaylistAccessException.cs
```

#### File Responsibilities

| File | Responsibility |
|---|---|
| `User.cs` | User entity. Properties: `Id`, `Username`, `Email`, `CreatedAt`. Seeded only. |
| `Song.cs` | Song entity. Properties: `Id`, `Title`, `Artist`, `DurationSeconds`. Seeded only. |
| `Playlist.cs` | Aggregate root. Properties: `Id`, `Name`, `IsPublic`, `OwnerId`, `CreatedAt`. Navigation: `ICollection<UserPlaylist>`. Business methods: `Rename`, `ChangePrivacy`, `AddSong`, `RemoveSong`, `CanBeViewedBy`. Ownership and privacy rules live here. |
| `UserPlaylist.cs` | Join entity linking `User` and `Playlist`. Composite key `(UserId, PlaylistId)`. Property: `AddedAt`. Decouples ownership from library membership. |
| `DomainException.cs` | Base custom exception for domain rule violations. |
| `PlaylistNotFoundException.cs` | Thrown when a playlist ID doesn't resolve. |
| `UserNotFoundException.cs` | Thrown when a user ID doesn't resolve. |
| `SongNotFoundException.cs` | Thrown when a song ID doesn't resolve. |
| `NotPlaylistOwnerException.cs` | Thrown when a non-owner attempts an owner-only action. |
| `PrivatePlaylistAccessException.cs` | Thrown when a non-owner tries to view a private playlist. |

---

### Application Layer — `PlaylistControl.Application`

```
PlaylistControl.Application/
├── Common/
│   └── Interfaces/
│       ├── IUserReadRepository.cs
│       ├── ISongReadRepository.cs
│       ├── IPlaylistReadRepository.cs
│       └── IPlaylistWriteRepository.cs
├── Features/
│   ├── Playlists/
│   │   ├── Commands/
│   │   │   ├── CreatePlaylist/
│   │   │   │   ├── CreatePlaylistCommand.cs
│   │   │   │   ├── CreatePlaylistCommandHandler.cs
│   │   │   │   └── CreatePlaylistCommandValidator.cs
│   │   │   ├── UpdatePlaylist/
│   │   │   │   ├── UpdatePlaylistCommand.cs
│   │   │   │   ├── UpdatePlaylistCommandHandler.cs
│   │   │   │   └── UpdatePlaylistCommandValidator.cs
│   │   │   ├── DeletePlaylist/
│   │   │   │   ├── DeletePlaylistCommand.cs
│   │   │   │   └── DeletePlaylistCommandHandler.cs
│   │   │   ├── AddSongToPlaylist/
│   │   │   │   ├── AddSongToPlaylistCommand.cs
│   │   │   │   ├── AddSongToPlaylistCommandHandler.cs
│   │   │   │   └── AddSongToPlaylistCommandValidator.cs
│   │   │   ├── RemoveSongFromPlaylist/
│   │   │   │   ├── RemoveSongFromPlaylistCommand.cs
│   │   │   │   ├── RemoveSongFromPlaylistCommandHandler.cs
│   │   │   │   └── RemoveSongFromPlaylistCommandValidator.cs
│   │   │   ├── AddPlaylistToUser/
│   │   │   │   ├── AddPlaylistToUserCommand.cs
│   │   │   │   └── AddPlaylistToUserCommandHandler.cs
│   │   │   └── RemovePlaylistFromUser/
│   │   │       ├── RemovePlaylistFromUserCommand.cs
│   │   │       └── RemovePlaylistFromUserCommandHandler.cs
│   │   └── Queries/
│   │       ├── GetAllPlaylists/
│   │       │   ├── GetAllPlaylistsQuery.cs
│   │       │   ├── GetAllPlaylistsQueryHandler.cs
│   │       │   └── PlaylistSummaryDto.cs
│   │       ├── GetMyPlaylists/
│   │       │   ├── GetMyPlaylistsQuery.cs
│   │       │   └── GetMyPlaylistsQueryHandler.cs
│   │       ├── GetUserPlaylists/
│   │       │   ├── GetUserPlaylistsQuery.cs
│   │       │   └── GetUserPlaylistsQueryHandler.cs
│   │       └── GetPlaylistSongs/
│   │           ├── GetPlaylistSongsQuery.cs
│   │           ├── GetPlaylistSongsQueryHandler.cs
│   │           └── SongDto.cs
│   ├── Songs/
│   │   └── Queries/
│   │       └── GetAllSongs/
│   │           ├── GetAllSongsQuery.cs
│   │           └── GetAllSongsQueryHandler.cs
│   └── Users/
│       └── Queries/
│           └── GetAllUsers/
│               ├── GetAllUsersQuery.cs
│               ├── GetAllUsersQueryHandler.cs
│               └── UserDto.cs
├── Behaviors/
│   └── ValidationBehavior.cs
└── DependencyInjection.cs
```

#### File Responsibilities

| File | Responsibility |
|---|---|
| `IUserReadRepository.cs` | Read-only user lookups: `GetByIdAsync`, `GetAllAsync`, `ExistsAsync`. |
| `ISongReadRepository.cs` | Read-only song lookups: `GetByIdAsync`, `GetAllAsync`, `ExistsAsync`. |
| `IPlaylistReadRepository.cs` | Read-only playlist queries: `GetByIdAsync`, `GetByIdWithSongsAsync`, `GetAllPublicAsync`, `GetByOwnerAsync`, `GetUserLibraryAsync(userId)`. **No `SaveChangesAsync`.** |
| `IPlaylistWriteRepository.cs` | Write-side persistence **and commit**: `AddAsync`, `DeleteAsync`, `GetByIdWithSongsForUpdateAsync`, `GetUserPlaylistEntryAsync`, `AddUserPlaylistEntryAsync`, `RemoveUserPlaylistEntryAsync`, **`SaveChangesAsync`**. |
| `CreatePlaylistCommand.cs` + Handler | Creates a `Playlist` owned by the caller; adds the owner's `UserPlaylist` entry. Injects `IPlaylistWriteRepository`. One `SaveChangesAsync` at the end. |
| `UpdatePlaylistCommand.cs` + Handler | Renames and/or changes privacy via domain methods. Owner-only. Injects `IPlaylistWriteRepository`. |
| `DeletePlaylistCommand.cs` + Handler | Owner → delete playlist and its `UserPlaylist` entries. Non-owner member → delete only their entry. One `SaveChangesAsync`. |
| `AddSongToPlaylistCommand.cs` + Handler | Owner-only. Loads playlist with songs, calls `Playlist.AddSong`. One `SaveChangesAsync`. |
| `RemoveSongFromPlaylistCommand.cs` + Handler | Owner-only. Calls `Playlist.RemoveSong`. One `SaveChangesAsync`. |
| `AddPlaylistToUserCommand.cs` + Handler | Adds the playlist to the caller's library. Rejects if caller is owner or if playlist is private and caller isn't owner. One `SaveChangesAsync`. |
| `RemovePlaylistFromUserCommand.cs` + Handler | Removes the playlist from the caller's library only. Throws if caller is owner. One `SaveChangesAsync`. |
| `GetAllPlaylistsQuery.cs` + Handler | Public playlists plus any the caller owns. Injects `IPlaylistReadRepository`. No commit. |
| `GetMyPlaylistsQuery.cs` + Handler | Playlists where `OwnerId == callerId`. Injects `IPlaylistReadRepository`. |
| `GetUserPlaylistsQuery.cs` + Handler | Playlists in another user's library; private entries filtered unless caller is the playlist's owner. |
| `GetPlaylistSongsQuery.cs` + Handler | Songs in a playlist. Throws `PrivatePlaylistAccessException` if private and caller has no access. |
| `GetAllSongsQuery.cs` + Handler | Static song catalog. Injects `ISongReadRepository`. |
| `GetAllUsersQuery.cs` + Handler | Static user catalog. Injects `IUserReadRepository`. |
| `PlaylistSummaryDto.cs` | Id, Name, IsPublic, OwnerId, OwnerUsername, SongCount. |
| `SongDto.cs` | Id, Title, Artist, DurationSeconds. |
| `UserDto.cs` | Id, Username, Email. |
| `ValidationBehavior.cs` | MediatR pipeline behavior running FluentValidation before handlers. |
| `DependencyInjection.cs` | `AddApplication()` — registers MediatR, validators, pipeline behavior. |

---

### Infrastructure Layer — `PlaylistControl.Infrastructure`

```
PlaylistControl.Infrastructure/
├── Persistence/
│   ├── Write/
│   │   ├── PlaylistWriteDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── PlaylistConfiguration.cs
│   │   │   └── UserPlaylistConfiguration.cs
│   │   └── Repositories/
│   │       └── PlaylistWriteRepository.cs
│   ├── Read/
│   │   ├── PlaylistReadDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── UserConfiguration.cs
│   │   │   ├── SongConfiguration.cs
│   │   │   ├── PlaylistConfiguration.cs
│   │   │   └── UserPlaylistConfiguration.cs
│   │   └── Repositories/
│   │       ├── PlaylistReadRepository.cs
│   │       ├── UserReadRepository.cs
│   │       └── SongReadRepository.cs
│   ├── Seed/
│   │   └── DatabaseSeeder.cs
│   └── Migrations/   # generated against the Write context
└── DependencyInjection.cs
```

#### File Responsibilities

| File | Responsibility |
|---|---|
| `PlaylistWriteDbContext.cs` | Write-side `DbContext`. DbSets: **only** `Playlists`, `UserPlaylists`. Users and Songs are seed-only and never written by the API, so they are absent here. This is the real CQRS separation. |
| `PlaylistWriteDbContext` Configurations | `PlaylistConfiguration.cs`, `UserPlaylistConfiguration.cs` — write-side EF configurations. |
| `PlaylistWriteRepository.cs` | Implements `IPlaylistWriteRepository`. Performs mutations and exposes `SaveChangesAsync` via the write context. Does **not** auto-save. |
| `PlaylistReadDbContext.cs` | Read-side `DbContext`. DbSets: `Users`, `Songs`, `Playlists`, `UserPlaylists`. Sets `QueryTrackingBehavior.NoTracking` in `OnConfiguring`. Overrides `SaveChanges` / `SaveChangesAsync` to throw `InvalidOperationException` — enforces read-only at the type level. |
| `PlaylistReadDbContext` Configurations | `UserConfiguration.cs`, `SongConfiguration.cs`, `PlaylistConfiguration.cs`, `UserPlaylistConfiguration.cs` — read-side EF configurations mirroring the schema. |
| `PlaylistReadRepository.cs` | Implements `IPlaylistReadRepository`. Pure read queries against the read context. |
| `UserReadRepository.cs` | Implements `IUserReadRepository`. Read-only queries against `Users`. |
| `SongReadRepository.cs` | Implements `ISongReadRepository`. Read-only queries against `Songs`. |
| `DatabaseSeeder.cs` | Seeds static Users and Songs via the **Write context** (source of truth). Because both contexts point at the same database, the read context sees the same rows. |
| `DependencyInjection.cs` | `AddInfrastructure(connectionString)` — registers both contexts (same connection string), all four repositories. |

---

### API Layer — `PlaylistControl.Api`

```
PlaylistControl.Api/
├── Controllers/
│   ├── PlaylistsController.cs
│   ├── SongsController.cs
│   └── UsersController.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Extensions/
│   └── HttpContextExtensions.cs
├── Program.cs
└── appsettings.json
```

#### File Responsibilities

| File | Responsibility |
|---|---|
| `PlaylistsController.cs` | Endpoints mapped to commands/queries. Each action is 3–5 lines: extract requester id, build command/query, `_mediator.Send(...)`. |
| `SongsController.cs` | `GET /api/songs` → `GetAllSongsQuery`. |
| `UsersController.cs` | `GET /api/users` → `GetAllUsersQuery`. |
| `ExceptionHandlingMiddleware.cs` | Maps domain exceptions → HTTP: `*NotFoundException` → 404, `NotPlaylistOwnerException` → 403, `PrivatePlaylistAccessException` → 403, `DomainException` → 400. |
| `HttpContextExtensions.cs` | Reads acting user from `X-User-Id` header. Placeholder for real auth. |
| `Program.cs` | Composes Application + Infrastructure, runs seeder, configures middleware pipeline. |
| `appsettings.json` | Connection string (LocalDB by default). **One** connection string — both contexts share it. |

#### Endpoint → Command/Query Mapping

| HTTP | Route | Handler | Side |
|---|---|---|---|
| `POST` | `/api/playlists` | `CreatePlaylistCommand` | Write |
| `PUT` | `/api/playlists/{id}` | `UpdatePlaylistCommand` | Write |
| `DELETE` | `/api/playlists/{id}` | `DeletePlaylistCommand` | Write |
| `POST` | `/api/playlists/{id}/songs` | `AddSongToPlaylistCommand` | Write |
| `DELETE` | `/api/playlists/{id}/songs/{songId}` | `RemoveSongFromPlaylistCommand` | Write |
| `POST` | `/api/users/me/playlists/{id}` | `AddPlaylistToUserCommand` | Write |
| `DELETE` | `/api/users/me/playlists/{id}` | `RemovePlaylistFromUserCommand` | Write |
| `GET` | `/api/playlists` | `GetAllPlaylistsQuery` | Read |
| `GET` | `/api/users/me/playlists` | `GetMyPlaylistsQuery` | Read |
| `GET` | `/api/users/{userId}/playlists` | `GetUserPlaylistsQuery` | Read |
| `GET` | `/api/playlists/{id}/songs` | `GetPlaylistSongsQuery` | Read |
| `GET` | `/api/songs` | `GetAllSongsQuery` | Read |
| `GET` | `/api/users` | `GetAllUsersQuery` | Read |

---

### Test Projects

#### `PlaylistControl.UnitTests`

```
PlaylistControl.UnitTests/
├── Domain/
│   ├── PlaylistTests.cs
│   └── UserPlaylistTests.cs
└── Application/
    └── Playlists/
        ├── Commands/
        │   ├── CreatePlaylistCommandHandlerTests.cs
        │   ├── UpdatePlaylistCommandHandlerTests.cs
        │   ├── DeletePlaylistCommandHandlerTests.cs
        │   ├── AddSongToPlaylistCommandHandlerTests.cs
        │   ├── RemoveSongFromPlaylistCommandHandlerTests.cs
        │   ├── AddPlaylistToUserCommandHandlerTests.cs
        │   └── RemovePlaylistFromUserCommandHandlerTests.cs
        └── Queries/
            ├── GetAllPlaylistsQueryHandlerTests.cs
            ├── GetMyPlaylistsQueryHandlerTests.cs
            ├── GetUserPlaylistsQueryHandlerTests.cs
            └── GetPlaylistSongsQueryHandlerTests.cs
```

#### `PlaylistControl.IntegrationTests`

```
PlaylistControl.IntegrationTests/
├── CustomWebApplicationFactory.cs
├── PlaylistsEndpointsTests.cs
├── SongsEndpointsTests.cs
└── UsersEndpointsTests.cs
```

---

### Packages Per Layer

| Project | Packages |
|---|---|
| **Domain** | *(none)* |
| **Application** | `MediatR`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions` |
| **Infrastructure** | `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design` |
| **UnitTests** | `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `Moq`, `AutoFixture`, `FluentAssertions` |
| **IntegrationTests** | `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `Microsoft.AspNetCore.Mvc.Testing` |

---

### Project References

| Project | References |
|---|---|
| Domain | *(none)* |
| Application | Domain |
| Infrastructure | Application, Domain |
| Api | Application, Infrastructure |
| UnitTests | Domain, Application |
| IntegrationTests | Api |

---


