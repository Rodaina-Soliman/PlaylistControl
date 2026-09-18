using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Features.Playlists.Commands.CreatePlaylist;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Commands.CreatePlaylist;

public class CreatePlaylistCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IUserReadRepository> _userReadRepository = new();
    private readonly Mock<IPlaylistWriteRepository> _playlistWriteRepository = new();
    private readonly CreatePlaylistCommandHandler _handler;

    public CreatePlaylistCommandHandlerTests()
    {
        _handler = new CreatePlaylistCommandHandler(
            _userReadRepository.Object,
            _playlistWriteRepository.Object);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ThrowsUserNotFoundException()
    {
        var command = _fixture.Create<CreatePlaylistCommand>();
        _userReadRepository
            .Setup(r => r.ExistsAsync(command.RequesterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UserNotFoundException>();
        _playlistWriteRepository.Verify(
            r => r.AddAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsPlaylistOwnerEntryAndSaves()
    {
        var command = _fixture.Create<CreatePlaylistCommand>();
        _userReadRepository
            .Setup(r => r.ExistsAsync(command.RequesterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Playlist? actualPlaylist = null;
        UserPlaylist? actualEntry = null;

        _playlistWriteRepository
            .Setup(r => r.AddAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()))
            .Callback<Playlist, CancellationToken>((p, _) => actualPlaylist = p)
            .Returns(Task.CompletedTask);

        _playlistWriteRepository
            .Setup(r => r.AddUserPlaylistEntryAsync(It.IsAny<UserPlaylist>(), It.IsAny<CancellationToken>()))
            .Callback<UserPlaylist, CancellationToken>((e, _) => actualEntry = e)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        actualPlaylist.Should().NotBeNull();
        actualPlaylist!.Name.Should().Be(command.Name);
        actualPlaylist.IsPublic.Should().Be(command.IsPublic);
        actualPlaylist.OwnerId.Should().Be(command.RequesterId);
        actualPlaylist.CreatedAt.Should().NotBe(default);

        actualEntry.Should().NotBeNull();
        actualEntry!.UserId.Should().Be(command.RequesterId);
        actualEntry.PlaylistId.Should().Be(actualPlaylist.Id);
        actualEntry.AddedAt.Should().NotBe(default);

        _playlistWriteRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}