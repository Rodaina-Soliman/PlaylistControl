using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Application.Features.Playlists.Queries.GetUserPlaylists;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetUserPlaylists;

public class GetUserPlaylistsQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IUserReadRepository> _userReadRepository = new();
    private readonly Mock<IPlaylistReadRepository> _playlistReadRepository = new();
    private readonly GetUserPlaylistsQueryHandler _handler;

    public GetUserPlaylistsQueryHandlerTests()
    {
        _handler = new GetUserPlaylistsQueryHandler(
            _userReadRepository.Object,
            _playlistReadRepository.Object);
    }

    private Playlist BuildPlaylist(Guid ownerId, bool isPublic, string ownerUsername)
    {
        return _fixture.Build<Playlist>()
            .Without(p => p.UserPlaylists)
            .Without(p => p.SongPlaylists)
            .With(p => p.OwnerId, ownerId)
            .With(p => p.IsPublic, isPublic)
            .With(p => p.Owner, new User { Id = ownerId, Username = ownerUsername })
            .Create();
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ThrowsUserNotFoundException()
    {
        var query = new GetUserPlaylistsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, 1);
        _userReadRepository
            .Setup(r => r.ExistsAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _handler.Awaiting(x => x.Handle(query, CancellationToken.None))
             .Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task Handle_EmptyPage_ReturnsEmptyItemsWithMetadata()
    {
        var query = new GetUserPlaylistsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, 1);
        _userReadRepository
            .Setup(r => r.ExistsAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _playlistReadRepository
            .Setup(r => r.GetUserLibraryAsync(query.UserId, query.RequesterId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(Array.Empty<Playlist>(), 1, 1, 0));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MapsSummaryFieldsCorrectly()
    {
        var targetUserId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var playlist = BuildPlaylist(Guid.NewGuid(), isPublic: true, "alice");
        playlist.SongPlaylists.Add(new SongPlaylist
        {
            SongId = Guid.NewGuid(),
            PlaylistId = playlist.Id,
            AddedAt = DateTime.UtcNow
        });

        var query = new GetUserPlaylistsQuery(requesterId, targetUserId, 1, 1);
        _userReadRepository
            .Setup(r => r.ExistsAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _playlistReadRepository
            .Setup(r => r.GetUserLibraryAsync(targetUserId, requesterId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(new[] { playlist }, 1, 1, 1));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Single().Should().BeEquivalentTo(new
        {
            playlist.Id,
            playlist.Name,
            playlist.IsPublic,
            playlist.OwnerId,
            OwnerUsername = "alice",
            SongCount = 1
        });
        result.TotalCount.Should().Be(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_LastPartialPage_ReturnsItemsAsGiven()
    {
        var targetUserId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var lastItem = BuildPlaylist(requesterId, isPublic: false, "bob");

        var query = new GetUserPlaylistsQuery(requesterId, targetUserId, 3, 2);
        _userReadRepository
            .Setup(r => r.ExistsAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _playlistReadRepository
            .Setup(r => r.GetUserLibraryAsync(targetUserId, requesterId, 3, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Playlist>(new[] { lastItem }, 3, 2, 5));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.Single().Id.Should().Be(lastItem.Id);
        result.Page.Should().Be(3);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }
}