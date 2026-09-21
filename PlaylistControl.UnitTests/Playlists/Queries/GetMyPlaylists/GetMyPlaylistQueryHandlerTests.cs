using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Application.Features.Playlists.Queries.GetMyPlaylists;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetMyPlaylists;

public class GetMyPlaylistsQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IPlaylistReadRepository> _playlistReadRepository = new();
    private readonly GetMyPlaylistsQueryHandler _handler;

    public GetMyPlaylistsQueryHandlerTests()
    {
        _handler = new GetMyPlaylistsQueryHandler(_playlistReadRepository.Object);
    }

    private Playlist BuildPlaylist(Guid ownerId, string ownerUsername, int songCount)
    {
        var playlist = _fixture.Build<Playlist>()
            .Without(p => p.UserPlaylists)
            .Without(p => p.SongPlaylists)
            .With(p => p.OwnerId, ownerId)
            .With(p => p.Owner, new User { Id = ownerId, Username = ownerUsername })
            .Create();

        for (var i = 0; i < songCount; i++)
        {
            playlist.SongPlaylists.Add(new SongPlaylist
            {
                SongId = Guid.NewGuid(),
                PlaylistId = playlist.Id,
                AddedAt = DateTime.UtcNow
            });
        }

        return playlist;
    }

    [Fact]
    public async Task Handle_EmptyPage_ReturnsEmptyItemsWithMetadata()
    {
        var requesterId = Guid.NewGuid();
        var query = new GetMyPlaylistsQuery(requesterId, 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetByOwnerAsync(requesterId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(Array.Empty<Playlist>(), 1, 1, 0));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MapsItemsAndMetadata()
    {
        var requesterId = Guid.NewGuid();
        var first = BuildPlaylist(requesterId, "alice", 1);
        var second = BuildPlaylist(requesterId, "alice", 4);
        var query = new GetMyPlaylistsQuery(requesterId, 1, 2);
        _playlistReadRepository
            .Setup(r => r.GetByOwnerAsync(requesterId, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(new[] { first, second }, 1, 2, 2));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(d => d.Id == first.Id && d.SongCount == 1);
        result.Items.Should().ContainSingle(d => d.Id == second.Id && d.SongCount == 4);
        result.Items.Should().OnlyContain(d => d.OwnerId == requesterId);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_LastPartialPage_ReturnsItemsAsGiven()
    {
        var requesterId = Guid.NewGuid();
        var lastItem = BuildPlaylist(requesterId, "alice", 0);
        var query = new GetMyPlaylistsQuery(requesterId, 3, 2);
        _playlistReadRepository
            .Setup(r => r.GetByOwnerAsync(requesterId, 3, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(new[] { lastItem }, 3, 2, 5));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Page.Should().Be(3);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }
}