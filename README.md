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
    ├── PrivatePlaylistAccessException.cs
    └── RedundantOperationException.cs
```

##### File Responsibilities

| File | Responsibility |
|---|---|
| `User.cs` | User entity. Properties: `Id`, `Username`, `Email`, `CreatedAt`. Navigation: `ICollection<UserPlaylist>`. Seeded only. |
| `Song.cs` | Song entity. Properties: `Id`, `Title`, `Artist`, `DurationSeconds`. Navigation: `ICollection<SongPlaylist>`. Seeded only. |
| `Playlist.cs` | Aggregate root. Properties: `Id`, `Name`, `IsPublic`, `OwnerId`, `CreatedAt`. Navigation: `Owner`, `ICollection<UserPlaylist>`, `ICollection<SongPlaylist>`. Plain data model — ownership and privacy rules are enforced in handlers, not here. |
| `UserPlaylist.cs` | Join entity linking `User` and `Playlist`. Composite key `(UserId, PlaylistId)`. Property: `AddedAt`. Navigation: `User`, `Playlist`. Decouples ownership from library membership. |
| `SongPlaylist.cs` | Join entity linking `Song` and `Playlist`. Composite key `(SongId, PlaylistId)`. Property: `AddedAt`. Navigation: `Song`, `Playlist`. |
| `DomainException.cs` | Base custom exception for domain rule violations. |
| `PlaylistNotFoundException.cs` | Thrown when a playlist ID doesn't resolve. Inherits `DomainException`. |
| `UserNotFoundException.cs` | Thrown when a user ID doesn't resolve. Inherits `DomainException`. |
| `SongNotFoundException.cs` | Thrown when a song ID doesn't resolve. Inherits `DomainException`. |
| `NotPlaylistOwnerException.cs` | Thrown when a non-owner attempts an owner-only action. Inherits `DomainException`. |
| `PrivatePlaylistAccessException.cs` | Thrown when a non-owner tries to view a private playlist. Inherits `DomainException`. |
| `RedundantOperationException.cs` | Thrown when an operation is redundant (e.g., adding an already-present entry, removing a non-present entry, owner attempting a member-only action). Inherits `DomainException`. |

---

### Application Layer — `PlaylistControl.Application`

```
PlaylistControl.Application/
├── Common/
│   ├── DTOs/
│   │   ├── PlaylistSummaryDto.cs
│   │   ├── SongDto.cs
│   │   └── UserDto.cs
│   ├── Interfaces/
│   │   ├── IUserReadRepository.cs
│   │   ├── ISongReadRepository.cs
│   │   ├── IPlaylistReadRepository.cs
│   │   └── IPlaylistWriteRepository.cs
│   └── Models/
│       ├── PagedResult.cs
│       └── PlaylistWithSongsPage.cs
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
│   │       │   └── GetAllPlaylistsQueryValidator.cs
│   │       ├── GetMyPlaylists/
│   │       │   ├── GetMyPlaylistsQuery.cs
│   │       │   ├── GetMyPlaylistsQueryHandler.cs
│   │       │   └── GetMyPlaylistsQueryValidator.cs
│   │       ├── GetUserPlaylists/
│   │       │   ├── GetUserPlaylistsQuery.cs
│   │       │   ├── GetUserPlaylistsQueryHandler.cs
│   │       │   └── GetUserPlaylistsQueryValidator.cs
│   │       └── GetPlaylistSongs/
│   │           ├── GetPlaylistSongsQuery.cs
│   │           ├── GetPlaylistSongsQueryHandler.cs
│   │           └── GetPlaylistSongsQueryValidator.cs
│   ├── Songs/
│   │   └── Queries/
│   │       └── GetAllSongs/
│   │           ├── GetAllSongsQuery.cs
│   │           ├── GetAllSongsQueryHandler.cs
│   │           └── GetAllSongsQueryValidator.cs
│   └── Users/
│       └── Queries/
│           └── GetAllUsers/
│               ├── GetAllUsersQuery.cs
│               ├── GetAllUsersQueryHandler.cs
│               └── GetAllUsersQueryValidator.cs
├── Behaviors/
│   └── ValidationBehavior.cs
└── DependencyInjection.cs
```

#### File Responsibilities

| File | Responsibility |
|---|---|
| `Common/DTOs/PlaylistSummaryDto.cs` | `Id`, `Name`, `IsPublic`, `OwnerId`, `OwnerUsername`, `SongCount`. Includes `PlaylistSummaryDtoExtensions.ToSummaryDto(this Playlist)` for projecting entities. |
| `Common/DTOs/SongDto.cs` | `Id`, `Title`, `Artist`, `DurationSeconds`. Includes `SongDtoExtensions.ToSongDto(this Song)`. |
| `Common/DTOs/UserDto.cs` | `Id`, `Username`, `Email`. Includes `UserDtoExtensions.ToUserDto(this User)`. |
| `Common/Models/PagedResult.cs` | Generic paged wrapper: `Items`, `Page`, `PageSize`, `TotalCount`, computed `TotalPages`. |
| `Common/Models/PlaylistWithSongsPage.cs` | Carries a playlist's access metadata (`Playlist?`) alongside a `PagedResult<Song>`. Returned by `GetPlaylistWithPagedSongsAsync`. |
| `Common/Interfaces/IUserReadRepository.cs` | Read-only user lookups: `GetAllAsync(page, pageSize)`, `ExistsAsync`. |
| `Common/Interfaces/ISongReadRepository.cs` | Read-only song lookups: `GetAllAsync(page, pageSize)`, `ExistsAsync`. |
| `Common/Interfaces/IPlaylistReadRepository.cs` | Read-only playlist queries: `GetByIdAsync`, `GetByOwnerAsync(page, pageSize)`, `GetUserLibraryAsync(userId, requesterId, page, pageSize)`, `GetPublicAndOwnedAsync(ownerId, page, pageSize)`, `GetPlaylistWithPagedSongsAsync(playlistId, page, pageSize)`, `IsInUserLibraryAsync(userId, playlistId)`. **No `SaveChangesAsync`.** |
| `Common/Interfaces/IPlaylistWriteRepository.cs` | Write-side persistence **and commit**: `AddAsync`, `DeleteAsync`, `GetByIdForUpdateAsync`, `GetByIdWithSongsForUpdateAsync`, `GetUserPlaylistEntryAsync`, `AddUserPlaylistEntryAsync`, `RemoveUserPlaylistEntryAsync`, `AddSongPlaylistEntryAsync`, `RemoveSongPlaylistEntryAsync`, **`SaveChangesAsync`**. |
| `CreatePlaylistCommand` + Handler + Validator | Creates a `Playlist` owned by the caller; adds the owner's `UserPlaylist` entry. Validates user existence. Returns `Guid`. One `SaveChangesAsync`. |
| `UpdatePlaylistCommand` + Handler + Validator | Renames and/or changes privacy. Owner-only. Validator requires at least one of `Name`/`IsPublic`. Returns `bool`. |
| `DeletePlaylistCommand` + Handler | Owner-only. Deletes the playlist (cascade removes its join rows). Non-owner → `NotPlaylistOwnerException`. Returns `bool`. |
| `AddSongToPlaylistCommand` + Handler + Validator | Owner-only. Validates song existence. Duplicate → `RedundantOperationException`. Inserts `SongPlaylist` row. Returns `bool`. |
| `RemoveSongFromPlaylistCommand` + Handler + Validator | Owner-only. Song not in playlist → `RedundantOperationException`. Removes `SongPlaylist` row. Returns `bool`. |
| `AddPlaylistToUserCommand` + Handler | Adds a playlist to a target user's library. Requester may be the target or the playlist owner. Private + requester ≠ owner → `PrivatePlaylistAccessException`. Target is owner or entry exists → `RedundantOperationException`. Returns `bool`. |
| `RemovePlaylistFromUserCommand` + Handler | Removes a playlist from the requester's own library. Requester is owner → `RedundantOperationException` ("delete it instead"). Entry missing → `RedundantOperationException`. Returns `bool`. |
| `GetAllPlaylistsQuery` + Handler + Validator | `PagedResult<PlaylistSummaryDto>` from `GetPublicAndOwnedAsync`. Validator enforces `RequesterId != empty`, `Page >= 1`, `PageSize >= 1`. |
| `GetMyPlaylistsQuery` + Handler + Validator | `PagedResult<PlaylistSummaryDto>` from `GetByOwnerAsync`. Same validator rules. |
| `GetUserPlaylistsQuery` + Handler + Validator | `PagedResult<PlaylistSummaryDto>` from `GetUserLibraryAsync(userId, requesterId, …)` (visibility filter in repo). Validates user existence. Same validator rules plus `UserId != empty`. |
| `GetPlaylistSongsQuery` + Handler + Validator | `PagedResult<SongDto>` from `GetPlaylistWithPagedSongsAsync`. `PlaylistNotFoundException` if missing; `PrivatePlaylistAccessException` if private and requester is neither owner nor library member. Same validator rules plus `PlaylistId != empty`. |
| `GetAllSongsQuery` + Handler + Validator | `PagedResult<SongDto>` from `ISongReadRepository.GetAllAsync(page, pageSize)`. Same validator rules. |
| `GetAllUsersQuery` + Handler + Validator | `PagedResult<UserDto>` from `IUserReadRepository.GetAllAsync(page, pageSize)`. Same validator rules. |
| `Behaviors/ValidationBehavior.cs` | MediatR pipeline behavior running FluentValidation before handlers. Throws `FluentValidation.ValidationException` on failure. |
| `DependencyInjection.cs` | `AddApplication()` — registers MediatR, validators, pipeline behavior. |

---

### Infrastructure Layer — `PlaylistControl.Infrastructure`

```
PlaylistControl.Infrastructure/
├── Persistence/
│   ├── Write/
│   │   ├── PlaylistWriteDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── UserConfiguration.cs
│   │   │   ├── SongConfiguration.cs
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
│   └── Seed/
│       └── DatabaseSeeder.cs
├── Migrations/   # generated against the Write context
└── DependencyInjection.cs
```

#### File Responsibilities

| File | Responsibility |
|---|---|
| `PlaylistWriteDbContext.cs` | Write-side `DbContext`. DbSets: `Playlists`, `UserPlaylists`, `SongPlaylists`. Applies only configurations whose namespace contains `Persistence.Write.Configurations`. Note: `User` and `Song` are seeded via `Set<User>()` / `Set<Song>()` (they resolve from the read-side configuration assembly, not a write-side config). |
| `Write/Configurations/UserConfiguration.cs` | Configures `User`: table `Users`, PK `Id`, `Username` required max 100, `Email` required max 200, `CreatedAt` required. |
| `Write/Configurations/SongConfiguration.cs` | Configures `Song`: table `Songs`, PK `Id`, `Title` required max 200, `Artist` required max 200, `DurationSeconds` required. |
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
| `PlaylistReadRepository.cs` | Implements `IPlaylistReadRepository`. `GetByIdAsync` (includes `Owner`), `GetByOwnerAsync` (paged, ordered by `Name` then `Id`), `GetUserLibraryAsync` (paged, filters visibility in the query using `requesterId`, ordered by `UserPlaylist.AddedAt` then `PlaylistId`), `GetPublicAndOwnedAsync` (paged, single `WHERE IsPublic OR OwnerId = @ownerId`, ordered by `Name` then `Id`), `GetPlaylistWithPagedSongsAsync` (loads playlist metadata + paged songs ordered by `SongPlaylist.AddedAt` then `SongId`; returns `(null, empty PagedResult<Song>)` when playlist not found), `IsInUserLibraryAsync`. No tracking. |
| `UserReadRepository.cs` | Implements `IUserReadRepository`. `GetAllAsync` (paged, ordered by `Username` then `Id`), `ExistsAsync`. |
| `SongReadRepository.cs` | Implements `ISongReadRepository`. `GetAllAsync` (paged, ordered by `Title` then `Id`), `ExistsAsync`. |
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
| `ExceptionHandlingMiddleware.cs` | Maps exceptions → HTTP: `*NotFoundException` → 404, `NotPlaylistOwnerException` → 403, `PrivatePlaylistAccessException` → 403, `FluentValidation.ValidationException` → 400 (with `errors` payload), `RedundantOperationException` → 400, `DomainException` → 400. `ValidationException` must be handled **before** the `DomainException` clause. |
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
| `POST` | `/api/users/me/playlists/{id}` | `AddPlaylistToUserCommand` (target = requester) | Write |
| `POST` | `/api/playlists/{id}/members` | `AddPlaylistToUserCommand` (target from body) | Write |
| `DELETE` | `/api/users/me/playlists/{id}` | `RemovePlaylistFromUserCommand` | Write |
| `GET` | `/api/playlists?page=&pageSize=` | `GetAllPlaylistsQuery` | Read |
| `GET` | `/api/users/me/playlists?page=&pageSize=` | `GetMyPlaylistsQuery` | Read |
| `GET` | `/api/users/{userId}/playlists?page=&pageSize=` | `GetUserPlaylistsQuery` | Read |
| `GET` | `/api/playlists/{id}/songs?page=&pageSize=` | `GetPlaylistSongsQuery` | Read |
| `GET` | `/api/songs?page=&pageSize=` | `GetAllSongsQuery` | Read |
| `GET` | `/api/users?page=&pageSize=` | `GetAllUsersQuery` | Read |

Paged read endpoints return `{ items, page, pageSize, totalCount, totalPages }`.

---

### Test Projects

#### `PlaylistControl.UnitTests`

Each command/query has its own folder containing its handler tests and, where applicable, its validator tests. Domain entity tests are omitted — the entities are plain data models with no behavior to test.

```
PlaylistControl.UnitTests/
├── Playlists/
│   ├── Commands/
│   │   ├── CreatePlaylist/
│   │   │   ├── CreatePlaylistCommandHandlerTests.cs
│   │   │   └── CreatePlaylistCommandValidatorTests.cs
│   │   ├── UpdatePlaylist/
│   │   │   ├── UpdatePlaylistCommandHandlerTests.cs
│   │   │   └── UpdatePlaylistCommandValidatorTests.cs
│   │   ├── DeletePlaylist/
│   │   │   └── DeletePlaylistCommandHandlerTests.cs
│   │   ├── AddSongToPlaylist/
│   │   │   ├── AddSongToPlaylistCommandHandlerTests.cs
│   │   │   └── AddSongToPlaylistCommandValidatorTests.cs
│   │   ├── RemoveSongFromPlaylist/
│   │   │   ├── RemoveSongFromPlaylistCommandHandlerTests.cs
│   │   │   └── RemoveSongFromPlaylistCommandValidatorTests.cs
│   │   ├── AddPlaylistToUser/
│   │   │   └── AddPlaylistToUserCommandHandlerTests.cs
│   │   └── RemovePlaylistFromUser/
│   │       └── RemovePlaylistFromUserCommandHandlerTests.cs
│   └── Queries/
│       ├── GetAllPlaylists/
│       │   ├── GetAllPlaylistsQueryHandlerTests.cs
│       │   └── GetAllPlaylistsQueryValidatorTests.cs
│       ├── GetMyPlaylists/
│       │   ├── GetMyPlaylistsQueryHandlerTests.cs
│       │   └── GetMyPlaylistsQueryValidatorTests.cs
│       ├── GetUserPlaylists/
│       │   ├── GetUserPlaylistsQueryHandlerTests.cs
│       │   └── GetUserPlaylistsQueryValidatorTests.cs
│       └── GetPlaylistSongs/
│           ├── GetPlaylistSongsQueryHandlerTests.cs
│           └── GetPlaylistSongsQueryValidatorTests.cs
├── Songs/
│   └── Queries/
│       └── GetAllSongs/
│           ├── GetAllSongsQueryHandlerTests.cs
│           └── GetAllSongsQueryValidatorTests.cs
└── Users/
    └── Queries/
        └── GetAllUsers/
            ├── GetAllUsersQueryHandlerTests.cs
            └── GetAllUsersQueryValidatorTests.cs
```

Test tooling: `xunit`, `Moq`, `AutoFixture`, `FluentAssertions` (8.x), and `FluentValidation.TestHelper`. Mock repositories are explicit `Mock<T>` fields on each test class. AutoFixture builds strip navigation properties (`Owner`, `UserPlaylists`, `SongPlaylists`) to avoid circular-reference graphs, then re-add only the navigation a specific test needs.

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
| **UnitTests** | `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `Moq`, `AutoFixture`, `FluentAssertions`, `coverlet.collector` |
| **IntegrationTests** | `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `Microsoft.AspNetCore.Mvc.Testing`, `FluentAssertions`, `Microsoft.EntityFrameworkCore.Sqlite` |
| **Api** | `Microsoft.EntityFrameworkCore.Design` |


---

### Project References

| Project | References |
|---|---|
| Domain | *(none)* |
| Application | Domain |
| Infrastructure | Application, Domain |
| Api | Application, Infrastructure |
| UnitTests | Application |
| IntegrationTests | Api |

---
