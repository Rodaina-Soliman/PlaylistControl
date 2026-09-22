using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace PlaylistControl.IntegrationTests
{
    public class SongsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SongsEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_Returns_Paged_Songs()
        {
            var response = await _client.GetAsync("/api/songs?page=1&pageSize=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            json.GetProperty("items").GetArrayLength().Should().BeGreaterThan(0);
            json.GetProperty("totalCount").GetInt32().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAll_Returns_400_On_Bad_Paging()
        {
            var response = await _client.GetAsync("/api/songs?page=0&pageSize=10");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}