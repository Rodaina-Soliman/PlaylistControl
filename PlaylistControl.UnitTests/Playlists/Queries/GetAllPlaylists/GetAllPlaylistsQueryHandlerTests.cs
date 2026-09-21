using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Application.Features.Playlists.Queries.GetAllPlaylists;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetAllPlaylists;

public class GetAllPlaylistsQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IPlaylistReadRepository> _playlistReadRepository = new();
    private readonly GetAllPlaylistsQueryHandler _handler;

    public GetAllPlaylistsQueryHandlerTests()
    {
        _handler = new GetAllPlaylistsQueryHandler(_playlistReadRepository.Object);
    }

    private Playlist BuildPlaylist(Guid ownerId, bool isPublic, string ownerUsername, int songCount)
    {
        var playlist = _fixture.Build<Playlist>()
            .Without(p => p.UserPlaylists)
            .Without(p => p.SongPlaylists)
            .With(p => p.OwnerId, ownerId)
            .With(p => p.IsPublic, isPublic)
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
        var query = new GetAllPlaylistsQuery(requesterId, 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetPublicAndOwnedAsync(requesterId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(Array.Empty<Playlist>(), 1, 1, 0));

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
        var requesterId = Guid.NewGuid();
        var playlist = BuildPlaylist(Guid.NewGuid(), isPublic: true, "alice", 2);
        var query = new GetAllPlaylistsQuery(requesterId, 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetPublicAndOwnedAsync(requesterId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(new[] { playlist }, 1, 1, 5));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.Single().Id.Should().Be(playlist.Id);
        result.Items.Single().OwnerUsername.Should().Be("alice");
        result.Items.Single().SongCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(5);
    }

    [Fact]
    public async Task Handle_PageBeyondRange_ReturnsEmptyItemsWithMetadata()
    {
        var requesterId = Guid.NewGuid();
        var query = new GetAllPlaylistsQuery(requesterId, 10, 1);
        _playlistReadRepository
            .Setup(r => r.GetPublicAndOwnedAsync(requesterId, 10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(Array.Empty<Playlist>(), 10, 1, 3));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Page.Should().Be(10);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }
}