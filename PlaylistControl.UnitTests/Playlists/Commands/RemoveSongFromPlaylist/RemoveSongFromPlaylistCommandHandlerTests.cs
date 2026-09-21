using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Features.Playlists.Commands.RemoveSongFromPlaylist;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Commands.RemoveSongFromPlaylist;

public class RemoveSongFromPlaylistCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IPlaylistWriteRepository> _playlistWriteRepository = new();
    private readonly RemoveSongFromPlaylistCommandHandler _handler;

    public RemoveSongFromPlaylistCommandHandlerTests()
    {
        _handler = new RemoveSongFromPlaylistCommandHandler(_playlistWriteRepository.Object);
    }

    private Playlist BuildTrackedPlaylist(Guid ownerId, params Guid[] songIds)
    {
        var playlist = _fixture.Build<Playlist>()
            .Without(p => p.Owner)
            .Without(p => p.UserPlaylists)
            .Without(p => p.SongPlaylists)
            .With(p => p.OwnerId, ownerId)
            .Create();

        foreach (var songId in songIds)
        {
            playlist.SongPlaylists.Add(new SongPlaylist
            {
                SongId = songId,
                PlaylistId = playlist.Id,
                AddedAt = DateTime.UtcNow
            });
        }

        return playlist;
    }

    [Fact]
    public async Task Handle_PlaylistNotFound_ThrowsPlaylistNotFoundException()
    {
        var command = _fixture.Create<RemoveSongFromPlaylistCommand>();
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist?)null);

        await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
             .Should().ThrowAsync<PlaylistNotFoundException>();
    }

    [Fact]
    public async Task Handle_RequesterIsNotOwner_ThrowsNotPlaylistOwnerException()
    {
        var command = _fixture.Create<RemoveSongFromPlaylistCommand>();
        var playlist = BuildTrackedPlaylist(Guid.NewGuid());
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
             .Should().ThrowAsync<NotPlaylistOwnerException>();
    }

    [Fact]
    public async Task Handle_SongNotInPlaylist_ThrowsRedundantOperationException()
    {
        var requesterId = Guid.NewGuid();
        var playlist = BuildTrackedPlaylist(requesterId);
        var command = new RemoveSongFromPlaylistCommand(requesterId, playlist.Id, Guid.NewGuid());
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<RedundantOperationException>();
    }

    [Fact]
    public async Task Handle_ValidRequest_RemovesEntryAndSaves()
    {
        var requesterId = Guid.NewGuid();
        var songId = Guid.NewGuid();
        var playlist = BuildTrackedPlaylist(requesterId, songId);
        var command = new RemoveSongFromPlaylistCommand(requesterId, playlist.Id, songId);
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _playlistWriteRepository.Verify(
            r => r.RemoveSongPlaylistEntryAsync(
                It.Is<SongPlaylist>(sp => sp.SongId == songId && sp.PlaylistId == playlist.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _playlistWriteRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}