using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Application.Features.Songs.Queries.GetAllSongs;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.UnitTests.Songs.Queries.GetAllSongs;

public class GetAllSongsQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<ISongReadRepository> _songReadRepository = new();
    private readonly GetAllSongsQueryHandler _handler;

    public GetAllSongsQueryHandlerTests()
    {
        _handler = new GetAllSongsQueryHandler(_songReadRepository.Object);
    }

    private Song BuildSong() =>
        _fixture.Build<Song>()
            .Without(s => s.SongPlaylists)
            .Create();

    [Fact]
    public async Task Handle_EmptyPage_ReturnsEmptyItemsWithMetadata()
    {
        var query = new GetAllSongsQuery(1, 1);
        _songReadRepository
            .Setup(r => r.GetAllAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Song>(Array.Empty<Song>(), 1, 1, 0));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_FirstPage_MapsItemsAndMetadata()
    {
        var song = BuildSong();
        var query = new GetAllSongsQuery(1, 1);
        _songReadRepository
            .Setup(r => r.GetAllAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Song>(new[] { song }, 1, 1, 3));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.Single().Id.Should().Be(song.Id);
        result.Items.Single().Title.Should().Be(song.Title);
        result.Items.Single().Artist.Should().Be(song.Artist);
        result.Items.Single().DurationSeconds.Should().Be(song.DurationSeconds);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_PageBeyondRange_ReturnsEmptyItemsWithMetadata()
    {
        var query = new GetAllSongsQuery(10, 1);
        _songReadRepository
            .Setup(r => r.GetAllAsync(10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Song>(Array.Empty<Song>(), 10, 1, 3));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Page.Should().Be(10);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }
}