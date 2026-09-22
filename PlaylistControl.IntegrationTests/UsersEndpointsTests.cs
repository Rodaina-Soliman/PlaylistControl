using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace PlaylistControl.IntegrationTests
{
    public class UsersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UsersEndpointsTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static HttpRequestMessage WithUser(HttpMethod method, string url, Guid userId, object? body = null)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Add("X-User-Id", userId.ToString());
            if (body is not null)
            {
                request.Content = JsonContent.Create(body);
            }
            return request;
        }

        [Fact]
        public async Task GetAll_Returns_Paged_Users()
        {
            var response = await _client.GetAsync("/api/users?page=1&pageSize=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            json.GetProperty("items").GetArrayLength().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAll_Returns_400_On_Bad_Paging()
        {
            var response = await _client.GetAsync("/api/users?page=0&pageSize=10");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMine_Returns_200()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, "/api/users/me/playlists?page=1&pageSize=10",
                CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetForUser_Returns_200_For_Known_User()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, $"/api/users/{CustomWebApplicationFactory.AliceId}/playlists?page=1&pageSize=10",
                CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetForUser_Returns_404_For_Unknown_User()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, $"/api/users/{Guid.NewGuid()}/playlists?page=1&pageSize=10",
                CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddToMyLibrary_Returns_204_When_Public_Playlist()
        {
            // Alice creates a public playlist.
            var create = await _client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", CustomWebApplicationFactory.AliceId,
                new { name = "Public", isPublic = true }));
            var playlistId = await create.Content.ReadFromJsonAsync<Guid>();

            // Bob adds it to his own library.
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/users/me/playlists/{playlistId}",
                CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AddToMyLibrary_Returns_403_For_Private_Playlist()
        {
            var create = await _client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", CustomWebApplicationFactory.AliceId,
                new { name = "Private", isPublic = false }));
            var playlistId = await create.Content.ReadFromJsonAsync<Guid>();

            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/users/me/playlists/{playlistId}",
                CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddToMyLibrary_Returns_400_When_Owner()
        {
            var create = await _client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", CustomWebApplicationFactory.AliceId,
                new { name = "Mine", isPublic = true }));
            var playlistId = await create.Content.ReadFromJsonAsync<Guid>();

            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/users/me/playlists/{playlistId}",
                CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveFromMyLibrary_Returns_204_When_Member()
        {
            var create = await _client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", CustomWebApplicationFactory.AliceId,
                new { name = "Shared", isPublic = true }));
            var playlistId = await create.Content.ReadFromJsonAsync<Guid>();

            await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/users/me/playlists/{playlistId}",
                CustomWebApplicationFactory.BobId));

            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete, $"/api/users/me/playlists/{playlistId}",
                CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task RemoveFromMyLibrary_Returns_400_When_Owner()
        {
            var create = await _client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", CustomWebApplicationFactory.AliceId,
                new { name = "Owned", isPublic = true }));
            var playlistId = await create.Content.ReadFromJsonAsync<Guid>();

            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete, $"/api/users/me/playlists/{playlistId}",
                CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveFromMyLibrary_Returns_400_When_Not_In_Library()
        {
            var create = await _client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", CustomWebApplicationFactory.AliceId,
                new { name = "NotMine", isPublic = true }));
            var playlistId = await create.Content.ReadFromJsonAsync<Guid>();

            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete, $"/api/users/me/playlists/{playlistId}",
                CustomWebApplicationFactory.CarolId));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}