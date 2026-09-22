using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Features.Playlists.Commands.RemovePlaylistFromUser;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Commands.RemovePlaylistFromUser
{
    public class RemovePlaylistFromUserCommandHandlerTests
    {
        private readonly Mock<IPlaylistWriteRepository> _writeRepository = new();
        private readonly IFixture _fixture;
        private readonly RemovePlaylistFromUserCommandHandler _handler;

        public RemovePlaylistFromUserCommandHandlerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _handler = new RemovePlaylistFromUserCommandHandler(_writeRepository.Object);
        }

        private Playlist BuildPlaylist(Guid ownerId)
            => _fixture.Build<Playlist>()
                .Without(p => p.Owner)
                .Without(p => p.UserPlaylists)
                .Without(p => p.SongPlaylists)
                .With(p => p.OwnerId, ownerId)
                .Create();

        [Fact]
        public async Task Handle_Removes_Entry_And_Saves_When_Valid()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var playlist = BuildPlaylist(ownerId);
            var entry = new UserPlaylist { UserId = requesterId, PlaylistId = playlist.Id };
            var command = new RemovePlaylistFromUserCommand(requesterId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _writeRepository.Setup(r => r.GetUserPlaylistEntryAsync(
                    requesterId, playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entry);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
            _writeRepository.Verify(r => r.RemoveUserPlaylistEntryAsync(
                entry, It.IsAny<CancellationToken>()), Times.Once);
            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Throws_When_Playlist_Missing()
        {
            var playlistId = Guid.NewGuid();
            var command = new RemovePlaylistFromUserCommand(Guid.NewGuid(), playlistId);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlistId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Playlist?)null);

            await Assert.ThrowsAsync<PlaylistNotFoundException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Throws_When_Requester_Is_Owner()
        {
            var ownerId = Guid.NewGuid();
            var playlist = BuildPlaylist(ownerId);
            var command = new RemovePlaylistFromUserCommand(ownerId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);

            await Assert.ThrowsAsync<RedundantOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Throws_When_Entry_Not_Present()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var playlist = BuildPlaylist(ownerId);
            var command = new RemovePlaylistFromUserCommand(requesterId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _writeRepository.Setup(r => r.GetUserPlaylistEntryAsync(
                    requesterId, playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserPlaylist?)null);

            await Assert.ThrowsAsync<RedundantOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}