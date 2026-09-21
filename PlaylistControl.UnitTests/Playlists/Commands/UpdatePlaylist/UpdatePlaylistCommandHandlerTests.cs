using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Features.Playlists.Commands.UpdatePlaylist;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IPlaylistWriteRepository> _playlistWriteRepository = new();
    private readonly UpdatePlaylistCommandHandler _handler;

    public UpdatePlaylistCommandHandlerTests()
    {
        _handler = new UpdatePlaylistCommandHandler(_playlistWriteRepository.Object);
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
        var command = _fixture.Create<UpdatePlaylistCommand>();
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist?)null);

        await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
             .Should().ThrowAsync<PlaylistNotFoundException>();
    }

    [Fact]
    public async Task Handle_RequesterIsNotOwner_ThrowsNotPlaylistOwnerException()
    {
        var command = _fixture.Create<UpdatePlaylistCommand>();
        var playlist = BuildPlaylist(Guid.NewGuid());
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotPlaylistOwnerException>();

    }

    [Fact]
    public async Task Handle_OnlyNameProvided_UpdatesNameAndSaves()
    {
        var requesterId = Guid.NewGuid();
        var playlist = BuildPlaylist(requesterId);
        var originalPrivacy = playlist.IsPublic;
        var command = new UpdatePlaylistCommand(requesterId, playlist.Id, "Updated Name", null);
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        playlist.Name.Should().Be("Updated Name");
        playlist.IsPublic.Should().Be(originalPrivacy);
        _playlistWriteRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_OnlyPrivacyProvided_UpdatesPrivacyAndSaves()
    {
        var requesterId = Guid.NewGuid();
        var playlist = BuildPlaylist(requesterId);
        playlist.IsPublic = true;
        var originalName = playlist.Name;
        var command = new UpdatePlaylistCommand(requesterId, playlist.Id, null, false);
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        playlist.Name.Should().Be(originalName);
        playlist.IsPublic.Should().BeFalse();
        _playlistWriteRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BothProvided_UpdatesBothAndSaves()
    {
        var requesterId = Guid.NewGuid();
        var playlist = BuildPlaylist(requesterId);
        playlist.IsPublic = true;
        var command = new UpdatePlaylistCommand(requesterId, playlist.Id, "Both", false);
        _playlistWriteRepository
            .Setup(r => r.GetByIdForUpdateAsync(command.PlaylistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        playlist.Name.Should().Be("Both");
        playlist.IsPublic.Should().BeFalse();
        _playlistWriteRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}