using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PlaylistControl.IntegrationTests
{
    public class PlaylistsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public PlaylistsEndpointsTests(CustomWebApplicationFactory factory)
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

        private static async Task<Guid> CreatePlaylistAsync(HttpClient client, Guid requesterId, string name, bool isPublic)
        {
            var response = await client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", requesterId, new { name, isPublic }));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            return id;
        }

        [Fact]
        public async Task Create_Returns_Created_With_Id()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "My Mix", true);
            id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Create_Returns_400_When_Name_Empty()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, "/api/playlists", CustomWebApplicationFactory.AliceId,
                new { name = "", isPublic = true }));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_Returns_400_When_Header_Missing()
        {
            var response = await _client.PostAsJsonAsync("/api/playlists", new { name = "x", isPublic = true });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_Returns_204_For_Owner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "Old", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Put, $"/api/playlists/{id}", CustomWebApplicationFactory.AliceId,
                new { name = "New", isPublic = false }));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Update_Returns_403_For_NonOwner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Put, $"/api/playlists/{id}", CustomWebApplicationFactory.BobId,
                new { name = "B" }));
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Returns_404_For_Missing()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Put, $"/api/playlists/{Guid.NewGuid()}", CustomWebApplicationFactory.AliceId,
                new { name = "x" }));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_Returns_400_When_No_Fields()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Put, $"/api/playlists/{id}", CustomWebApplicationFactory.AliceId,
                new { }));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_Returns_204_For_Owner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete, $"/api/playlists/{id}", CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_Returns_403_For_NonOwner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete, $"/api/playlists/{id}", CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Returns_404_For_Missing()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete, $"/api/playlists/{Guid.NewGuid()}", CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddSong_Returns_204_For_Owner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/songs", CustomWebApplicationFactory.AliceId,
                new { songId = CustomWebApplicationFactory.BohemianRhapsodyId }));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AddSong_Returns_400_On_Duplicate()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/songs", CustomWebApplicationFactory.AliceId,
                new { songId = CustomWebApplicationFactory.BohemianRhapsodyId }));
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/songs", CustomWebApplicationFactory.AliceId,
                new { songId = CustomWebApplicationFactory.BohemianRhapsodyId }));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddSong_Returns_404_For_Missing_Playlist()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{Guid.NewGuid()}/songs", CustomWebApplicationFactory.AliceId,
                new { songId = CustomWebApplicationFactory.BohemianRhapsodyId }));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddSong_Returns_404_For_Missing_Song()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/songs", CustomWebApplicationFactory.AliceId,
                new { songId = Guid.NewGuid() }));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddSong_Returns_403_For_NonOwner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/songs", CustomWebApplicationFactory.BobId,
                new { songId = CustomWebApplicationFactory.BohemianRhapsodyId }));
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RemoveSong_Returns_204_For_Owner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/songs", CustomWebApplicationFactory.AliceId,
                new { songId = CustomWebApplicationFactory.BohemianRhapsodyId }));
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete,
                $"/api/playlists/{id}/songs/{CustomWebApplicationFactory.BohemianRhapsodyId}",
                CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task RemoveSong_Returns_400_When_Not_Present()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete,
                $"/api/playlists/{id}/songs/{CustomWebApplicationFactory.BohemianRhapsodyId}",
                CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveSong_Returns_403_For_NonOwner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Delete,
                $"/api/playlists/{id}/songs/{CustomWebApplicationFactory.BohemianRhapsodyId}",
                CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAll_Returns_Paged_Payload()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, "/api/playlists?page=1&pageSize=10", CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            json.TryGetProperty("items", out _).Should().BeTrue();
            json.TryGetProperty("totalPages", out _).Should().BeTrue();
        }

        [Fact]
        public async Task GetAll_Returns_400_On_Bad_Paging()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, "/api/playlists?page=0&pageSize=10", CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetSongs_Returns_200_For_Public_Playlist()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, $"/api/playlists/{id}/songs?page=1&pageSize=10",
                CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSongs_Returns_403_For_Private_NonMember()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", false);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, $"/api/playlists/{id}/songs?page=1&pageSize=10",
                CustomWebApplicationFactory.BobId));
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetSongs_Returns_404_For_Missing_Playlist()
        {
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Get, $"/api/playlists/{Guid.NewGuid()}/songs?page=1&pageSize=10",
                CustomWebApplicationFactory.AliceId));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddMember_Returns_204_When_Owner_Adds_Target()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", false);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/members", CustomWebApplicationFactory.AliceId,
                new { userId = CustomWebApplicationFactory.BobId }));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AddMember_Returns_403_When_NonOwner_Adds_To_Private()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", false);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/members", CustomWebApplicationFactory.CarolId,
                new { userId = CustomWebApplicationFactory.BobId }));
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddMember_Returns_400_When_Target_Is_Owner()
        {
            var id = await CreatePlaylistAsync(_client, CustomWebApplicationFactory.AliceId, "A", true);
            var response = await _client.SendAsync(WithUser(
                HttpMethod.Post, $"/api/playlists/{id}/members", CustomWebApplicationFactory.AliceId,
                new { userId = CustomWebApplicationFactory.AliceId }));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}