using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Features.Playlists.Commands.DeletePlaylist;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Commands.DeletePlaylist;

public class DeletePlaylistCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IPlaylistWriteRepository> _playlistWriteRepository = new();
    private readonly DeletePlaylistCommandHandler _handler;

    public DeletePlaylistCommandHandlerTests()
    {
        _handler = new DeletePlaylistCommandHandler(_playlistWriteRepository.Object);
    }

    private Playlist BuildPlaylist(Guid ownerId) =>
        _fixture.Build<Playlist>()
            .Without(p => p.Owner)
            .Without(p => p.UserPlaylists)
            .Without(p => p.SongPlaylists)
            .With(p => p.OwnerId, ownerId)
            .Create();

    [Fact]
    public async Task Handle_PlaylistNotFound_ThrowsPlaylistNotFoundException()
    {
        var command = _fixture.Create<DeletePlaylistCommand>();
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist?)null);

        await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<PlaylistNotFoundException>();
    }

    [Fact]
    public async Task Handle_RequesterIsNotOwner_ThrowsNotPlaylistOwnerException()
    {
        var command = _fixture.Create<DeletePlaylistCommand>();
        var playlist = BuildPlaylist(Guid.NewGuid());
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotPlaylistOwnerException>();

        _playlistWriteRepository.Verify(
            r => r.DeleteAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_RequesterIsOwner_DeletesPlaylistAndSaves()
    {
        var requesterId = Guid.NewGuid();
        var playlist = BuildPlaylist(requesterId);
        var command = new DeletePlaylistCommand(requesterId, playlist.Id);
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _playlistWriteRepository.Verify(
            r => r.DeleteAsync(playlist, It.IsAny<CancellationToken>()),
            Times.Once);
        _playlistWriteRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}