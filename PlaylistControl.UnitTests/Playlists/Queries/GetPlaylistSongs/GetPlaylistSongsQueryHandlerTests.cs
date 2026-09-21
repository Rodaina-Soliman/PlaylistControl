using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Application.Features.Playlists.Queries.GetPlaylistSongs;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetPlaylistSongs;

public class GetPlaylistSongsQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IPlaylistReadRepository> _playlistReadRepository = new();
    private readonly GetPlaylistSongsQueryHandler _handler;

    public GetPlaylistSongsQueryHandlerTests()
    {
        _handler = new GetPlaylistSongsQueryHandler(_playlistReadRepository.Object);
    }

    private Playlist BuildPlaylistMetadata(Guid ownerId, bool isPublic)
    {
        var playlist = _fixture.Build<Playlist>()
            .Without(p => p.Owner)
            .Without(p => p.UserPlaylists)
            .Without(p => p.SongPlaylists)
            .With(p => p.OwnerId, ownerId)
            .With(p => p.IsPublic, isPublic)
            .Create();

        playlist.Owner = null!;
        playlist.UserPlaylists = new List<UserPlaylist>();
        playlist.SongPlaylists = new List<SongPlaylist>();

        return playlist;
    }

    private Song BuildSong()
    {
        var song = _fixture.Build<Song>()
            .Without(s => s.SongPlaylists)
            .Create();

        song.SongPlaylists = new List<SongPlaylist>();

        return song;
    }

    private static PlaylistWithSongsPage Page(
        Playlist? playlist, IEnumerable<Song> songs, int page, int pageSize, int totalCount)
        => new(playlist, new PagedResult<Song>(songs.ToList(), page, pageSize, totalCount));

    [Fact]
    public async Task Handle_PlaylistNotFound_ThrowsPlaylistNotFoundException()
    {
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetPlaylistWithPagedSongsAsync(query.PlaylistId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Page(null, Array.Empty<Song>(), 1, 1, 0));

        await _handler.Awaiting(x => x.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<PlaylistNotFoundException>();
    }

    [Fact]
    public async Task Handle_PrivatePlaylistRequesterNotOwnerNotMember_ThrowsPrivatePlaylistAccessException()
    {
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, 1);
        var playlist = BuildPlaylistMetadata(Guid.NewGuid(), isPublic: false);
        _playlistReadRepository
            .Setup(r => r.GetPlaylistWithPagedSongsAsync(query.PlaylistId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Page(playlist, Array.Empty<Song>(), 1, 1, 0));
        _playlistReadRepository
            .Setup(r => r.IsInUserLibraryAsync(query.RequesterId, query.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _handler.Awaiting(x => x.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<PrivatePlaylistAccessException>();
    }

    [Fact]
    public async Task Handle_PrivatePlaylistRequesterIsOwner_ReturnsPage()
    {
        var requesterId = Guid.NewGuid();
        var song = BuildSong();
        var playlist = BuildPlaylistMetadata(requesterId, isPublic: false);
        var query = new GetPlaylistSongsQuery(requesterId, playlist.Id, 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetPlaylistWithPagedSongsAsync(query.PlaylistId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Page(playlist, new[] { song }, 1, 1, 1));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.Single().Id.Should().Be(song.Id);
        result.TotalCount.Should().Be(1);
        _playlistReadRepository.Verify(
            r => r.IsInUserLibraryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PrivatePlaylistRequesterIsMember_ReturnsPage()
    {
        var requesterId = Guid.NewGuid();
        var song = BuildSong();
        var playlist = BuildPlaylistMetadata(Guid.NewGuid(), isPublic: false);
        var query = new GetPlaylistSongsQuery(requesterId, playlist.Id, 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetPlaylistWithPagedSongsAsync(query.PlaylistId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Page(playlist, new[] { song }, 1, 1, 1));
        _playlistReadRepository
            .Setup(r => r.IsInUserLibraryAsync(requesterId, playlist.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.Single().Id.Should().Be(song.Id);
    }

    [Fact]
    public async Task Handle_PublicPlaylist_ReturnsPageWithoutMembershipCheck()
    {
        var song = BuildSong();
        var playlist = BuildPlaylistMetadata(Guid.NewGuid(), isPublic: true);
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), playlist.Id, 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetPlaylistWithPagedSongsAsync(query.PlaylistId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Page(playlist, new[] { song }, 1, 1, 1));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        _playlistReadRepository.Verify(
            r => r.IsInUserLibraryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyPage_ReturnsEmptyItemsWithMetadata()
    {
        var playlist = BuildPlaylistMetadata(Guid.NewGuid(), isPublic: true);
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), playlist.Id, 1, 1);
        _playlistReadRepository
            .Setup(r => r.GetPlaylistWithPagedSongsAsync(query.PlaylistId, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Page(playlist, Array.Empty<Song>(), 1, 1, 0));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MultipleSongs_MapsAllFields()
    {
        var first = BuildSong();
        var second = BuildSong();
        var playlist = BuildPlaylistMetadata(Guid.NewGuid(), isPublic: true);
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), playlist.Id, 1, 2);
        _playlistReadRepository
            .Setup(r => r.GetPlaylistWithPagedSongsAsync(query.PlaylistId, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Page(playlist, new[] { first, second }, 1, 2, 2));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(d => d.Id == first.Id && d.Title == first.Title && d.Artist == first.Artist && d.DurationSeconds == first.DurationSeconds);
        result.Items.Should().ContainSingle(d => d.Id == second.Id && d.Title == second.Title && d.Artist == second.Artist && d.DurationSeconds == second.DurationSeconds);
        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);
    }
}