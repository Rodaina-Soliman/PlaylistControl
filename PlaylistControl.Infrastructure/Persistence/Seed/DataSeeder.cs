using Microsoft.EntityFrameworkCore;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Infrastructure.Persistence.Write;

namespace PlaylistControl.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Seeds static Users and Songs into the database
    /// </summary>
    public static class DatabaseSeeder
    {
        // Users
        private static readonly Guid AliceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid BobId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid CarolId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Songs
        private static readonly Guid BohemianRhapsodyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid HotelCaliforniaId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        private static readonly Guid StairwayToHeavenId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        private static readonly Guid ImagineId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        private static readonly Guid SmellsLikeTeenSpiritId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

        /// <summary>
        /// Seeds users and songs
        /// </summary>
        /// <param name="context">The write-side DbContext used to persist seed data</param>
        public static async Task SeedAsync(PlaylistWriteDbContext context)
        {
            if (!await context.Set<User>().AnyAsync())
            {
                await context.Set<User>().AddRangeAsync(BuildUsers());
            }

            if (!await context.Set<Song>().AnyAsync())
            {
                await context.Set<Song>().AddRangeAsync(BuildSongs());
            }

            await context.SaveChangesAsync();

        }

        /// <summary>
        /// Builds the static list of seed Users
        /// </summary>
        /// <returns>List of Users to seed</returns>
        private static List<User> BuildUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = AliceId,
                    Username = "alice",
                    Email = "alice@example.com",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = BobId,
                    Username = "bob",
                    Email = "bob@example.com",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = CarolId,
                    Username = "carol",
                    Email = "carol@example.com",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        /// <summary>
        /// Builds the static list of seed Songs
        /// </summary>
        /// <returns>List of Songs to seed</returns>
        private static List<Song> BuildSongs()
        {
            return new List<Song>
            {
                new Song
                {
                    Id = BohemianRhapsodyId,
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    DurationSeconds = 354
                },
                new Song
                {
                    Id = HotelCaliforniaId,
                    Title = "Hotel California",
                    Artist = "Eagles",
                    DurationSeconds = 391
                },
                new Song
                {
                    Id = StairwayToHeavenId,
                    Title = "Stairway to Heaven",
                    Artist = "Led Zeppelin",
                    DurationSeconds = 482
                },
                new Song
                {
                    Id = ImagineId,
                    Title = "Imagine",
                    Artist = "John Lennon",
                    DurationSeconds = 183
                },
                new Song
                {
                    Id = SmellsLikeTeenSpiritId,
                    Title = "Smells Like Teen Spirit",
                    Artist = "Nirvana",
                    DurationSeconds = 301
                }
            };
        }
    }
}