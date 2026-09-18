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
│   ├── UserPlaylist.cs
│   └── SongPlaylist.cs
└── Exceptions/
    ├── DomainException.cs
    ├── PlaylistNotFoundException.cs
    ├── UserNotFoundException.cs
    ├── SongNotFoundException.cs
    ├── NotPlaylistOwnerException.cs
    └── PrivatePlaylistAccessException.cs
```

##### File Responsibilities

| File | Responsibility |
|---|---|
| `User.cs` | User entity. Properties: `Id`, `Username`, `Email`, `CreatedAt`. Navigation: `ICollection<UserPlaylist>`. Seeded only. |
| `Song.cs` | Song entity. Properties: `Id`, `Title`, `Artist`, `DurationSeconds`. Navigation: `ICollection<SongPlaylist>`. Seeded only. |
| `Playlist.cs` | Aggregate root. Properties: `Id`, `Name`, `IsPublic`, `OwnerId`, `CreatedAt`. Navigation: `Owner`, `ICollection<UserPlaylist>`, `ICollection<SongPlaylist>`. Ownership and privacy rules are enforced in handlers, not here. |
| `UserPlaylist.cs` | Join entity linking `User` and `Playlist`. Composite key `(UserId, PlaylistId)`. Property: `AddedAt`. Navigation: `User`, `Playlist`. Decouples ownership from library membership. |
| `SongPlaylist.cs` | Join entity linking `Song` and `Playlist`. Composite key `(SongId, PlaylistId)`. Property: `AddedAt`. Navigation: `Song`, `Playlist`. |
| `DomainException.cs` | Base custom exception for domain rule violations. |
| `PlaylistNotFoundException.cs` | Thrown when a playlist ID doesn't resolve. Inherits `Exception` (not `DomainException`). |
| `UserNotFoundException.cs` | Thrown when a user ID doesn't resolve. Inherits `DomainException`. |
| `SongNotFoundException.cs` | Thrown when a song ID doesn't resolve. Inherits `DomainException`. |
| `NotPlaylistOwnerException.cs` | Thrown when a non-owner attempts an owner-only action. Inherits `DomainException`. |
| `PrivatePlaylistAccessException.cs` | Thrown when a non-owner tries to view a private playlist. Inherits `DomainException`. |

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
│   │   │   ├── UserPlaylistConfiguration.cs
│   │   │   └── SongPlaylistConfiguration.cs
│   │   └── Repositories/
│   │       └── PlaylistWriteRepository.cs
│   ├── Read/
│   │   ├── PlaylistReadDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── UserConfiguration.cs
│   │   │   ├── SongConfiguration.cs
│   │   │   ├── PlaylistConfiguration.cs
│   │   │   ├── UserPlaylistConfiguration.cs
│   │   │   └── SongPlaylistConfiguration.cs
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
| `PlaylistWriteDbContext.cs` | Write-side `DbContext`. DbSets: `Playlists`, `UserPlaylists`, `SongPlaylists`. Applies only configurations whose namespace contains `Persistence.Write.Configurations`. Note: `User` and `Song` are seeded via `Set<User>()` / `Set<Song>()` (they resolve from the read-side configuration assembly, not a write-side config). |
| `Write/Configurations/PlaylistConfiguration.cs` | Configures `Playlist`: table `Playlists`, PK `Id`, `Name` required max 200, `IsPublic` required, `CreatedAt` required. `Owner` → many with `OwnerId` FK, `DeleteBehavior.Restrict`. `UserPlaylists` and `SongPlaylists` cascades. |
| `Write/Configurations/UserPlaylistConfiguration.cs` | Configures `UserPlaylist`: table `UserPlaylists`, composite PK `(UserId, PlaylistId)`, `AddedAt` required, `User` → many with `UserId` FK, `DeleteBehavior.Restrict`. |
| `Write/Configurations/SongPlaylistConfiguration.cs` | Configures `SongPlaylist`: table `SongPlaylists`, composite PK `(SongId, PlaylistId)`, `AddedAt` required, `Song` → many with `SongId` FK, `DeleteBehavior.Restrict`. |
| `PlaylistWriteRepository.cs` | Implements `IPlaylistWriteRepository`. Methods: `AddAsync`, `DeleteAsync`, `GetByIdWithSongsForUpdateAsync`, `GetByIdForUpdateAsync`, `GetUserPlaylistEntryAsync`, `AddUserPlaylistEntryAsync`, `RemoveUserPlaylistEntryAsync`, `AddSongPlaylistEntryAsync`, `RemoveSongPlaylistEntryAsync`, `SaveChangesAsync`. Does **not** auto-save — handlers call `SaveChangesAsync` explicitly. |
| `PlaylistReadDbContext.cs` | Read-side `DbContext`. DbSets: `Users`, `Songs`, `Playlists`, `UserPlaylists`, `SongPlaylists`. Sets `QueryTrackingBehavior.NoTracking` in `OnConfiguring`. Applies only configurations whose namespace contains `Persistence.Read.Configurations`. Overrides `SaveChanges` / `SaveChangesAsync` to throw `InvalidOperationException` — enforces read-only at the type level. |
| `Read/Configurations/UserConfiguration.cs` | Configures `User`: table `Users`, PK `Id`, `Username` required max 100, `Email` required max 200, `CreatedAt` required. |
| `Read/Configurations/SongConfiguration.cs` | Configures `Song`: table `Songs`, PK `Id`, `Title` required max 200, `Artist` required max 200, `DurationSeconds` required. |
| `Read/Configurations/PlaylistConfiguration.cs` | Mirrors the write-side `Playlist` config (table, PK, property constraints, `Owner` FK with `Restrict`). Join navigations are mapped without explicit delete behavior. |
| `Read/Configurations/UserPlaylistConfiguration.cs` | Mirrors the write-side `UserPlaylist` config: composite PK `(UserId, PlaylistId)`, `AddedAt` required, `User` → many, `Playlist` → many. |
| `Read/Configurations/SongPlaylistConfiguration.cs` | Mirrors the write-side `SongPlaylist` config: composite PK `(SongId, PlaylistId)`, `AddedAt` required, `Song` → many, `Playlist` → many. |
| `PlaylistReadRepository.cs` | Implements `IPlaylistReadRepository`. Methods: `GetByIdAsync` (includes `Owner`), `GetByIdWithSongsAsync` (includes `SongPlaylists.Song`), `GetAllPublicAsync` (public only, includes `Owner` + `SongPlaylists`), `GetByOwnerAsync` (includes `Owner` + `SongPlaylists`), `GetUserLibraryAsync` (via `UserPlaylists`, projects to `Playlist` with `Owner` + `SongPlaylists`), `ExistsAsync`. No tracking. |
| `UserReadRepository.cs` | Implements `IUserReadRepository`. Methods: `GetByIdAsync`, `GetAllAsync`, `ExistsAsync`. Read-only queries against `Users`. |
| `SongReadRepository.cs` | Implements `ISongReadRepository`. Methods: `GetByIdAsync`, `GetAllAsync`, `ExistsAsync`. Read-only queries against `Songs`. |
| `Seed/DatabaseSeeder.cs` | Static seeder. Seeds 3 Users (alice, bob, carol) and 5 Songs (Bohemian Rhapsody, Hotel California, Stairway to Heaven, Imagine, Smells Like Teen Spirit) via `PlaylistWriteDbContext` using `Set<User>()` / `Set<Song>()`, guarded by `AnyAsync()` checks. Because both contexts point at the same database, the read context sees the same rows. |
| `DependencyInjection.cs` | `AddInfrastructure(services, configuration)` — reads `DefaultConnection` (throws `InvalidOperationException` if missing), registers `PlaylistWriteDbContext` and `PlaylistReadDbContext` (both `UseSqlServer` with the same connection string), and registers `IPlaylistWriteRepository`, `IPlaylistReadRepository`, `IUserReadRepository`, `ISongReadRepository` as scoped. |

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


