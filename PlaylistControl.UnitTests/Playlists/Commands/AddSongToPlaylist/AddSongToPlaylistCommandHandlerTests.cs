using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Features.Playlists.Commands.AddSongToPlaylist;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Commands.AddSongToPlaylist;

public class AddSongToPlaylistCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<ISongReadRepository> _songReadRepository = new();
    private readonly Mock<IPlaylistWriteRepository> _playlistWriteRepository = new();
    private readonly AddSongToPlaylistCommandHandler _handler;

    public AddSongToPlaylistCommandHandlerTests()
    {
        _handler = new AddSongToPlaylistCommandHandler(
            _songReadRepository.Object,
            _playlistWriteRepository.Object);
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
        var command = _fixture.Create<AddSongToPlaylistCommand>();
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PlaylistNotFoundException>();
    }

    [Fact]
    public async Task Handle_RequesterIsNotOwner_ThrowsNotPlaylistOwnerException()
    {
        var command = _fixture.Create<AddSongToPlaylistCommand>();
        var playlist = BuildTrackedPlaylist(Guid.NewGuid());
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotPlaylistOwnerException>();
    }

    [Fact]
    public async Task Handle_SongNotFound_ThrowsSongNotFoundException()
    {
        var requesterId = Guid.NewGuid();
        var playlist = BuildTrackedPlaylist(requesterId);
        var command = new AddSongToPlaylistCommand(requesterId, playlist.Id, Guid.NewGuid());
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);
        _songReadRepository
            .Setup(r => r.ExistsAsync(command.SongId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<SongNotFoundException>();
    }

    [Fact]
    public async Task Handle_SongAlreadyInPlaylist_ThrowsRedundantOperationException()
    {
        var requesterId = Guid.NewGuid();
        var songId = Guid.NewGuid();
        var playlist = BuildTrackedPlaylist(requesterId, songId);
        var command = new AddSongToPlaylistCommand(requesterId, playlist.Id, songId);
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);
        _songReadRepository
            .Setup(r => r.ExistsAsync(command.SongId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<RedundantOperationException>();
        _playlistWriteRepository.Verify(
            r => r.AddSongPlaylistEntryAsync(It.IsAny<SongPlaylist>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsSongEntryAndSaves()
    {
        var requesterId = Guid.NewGuid();
        var playlist = BuildTrackedPlaylist(requesterId);
        var command = new AddSongToPlaylistCommand(requesterId, playlist.Id, Guid.NewGuid());
        _playlistWriteRepository
            .Setup(r => r.GetByIdWithSongsForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);
        _songReadRepository
            .Setup(r => r.ExistsAsync(command.SongId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        SongPlaylist? capturedEntry = null;
        _playlistWriteRepository
            .Setup(r => r.AddSongPlaylistEntryAsync(It.IsAny<SongPlaylist>(), It.IsAny<CancellationToken>()))
            .Callback<SongPlaylist, CancellationToken>((e, _) => capturedEntry = e)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        capturedEntry.Should().NotBeNull();
        capturedEntry!.SongId.Should().Be(command.SongId);
        capturedEntry.PlaylistId.Should().Be(command.PlaylistId);
        capturedEntry.AddedAt.Should().NotBe(default);
        _playlistWriteRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}