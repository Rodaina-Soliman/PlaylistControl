using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Features.Playlists.Commands.AddPlaylistToUser;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.UnitTests.Playlists.Commands.AddPlaylistToUser
{
    public class AddPlaylistToUserCommandHandlerTests
    {
        private readonly Mock<IPlaylistWriteRepository> _writeRepository = new();
        private readonly Mock<IUserReadRepository> _userReadRepository = new();
        private readonly IFixture _fixture;
        private readonly AddPlaylistToUserCommandHandler _handler;

        public AddPlaylistToUserCommandHandlerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _handler = new AddPlaylistToUserCommandHandler(
                _writeRepository.Object, _userReadRepository.Object);
        }

        private Playlist BuildPlaylist(bool isPublic, Guid ownerId)
        {
            var playlist = _fixture.Build<Playlist>()
                .Without(p => p.Owner)
                .Without(p => p.UserPlaylists)
                .Without(p => p.SongPlaylists)
                .With(p => p.IsPublic, isPublic)
                .With(p => p.OwnerId, ownerId)
                .Create();
            return playlist;
        }

        [Fact]
        public async Task Handle_Adds_Entry_And_Saves_When_Valid()
        {
            var ownerId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var playlist = BuildPlaylist(isPublic: true, ownerId: ownerId);
            var command = new AddPlaylistToUserCommand(targetId, targetId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _userReadRepository.Setup(r => r.ExistsAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _writeRepository.Setup(r => r.GetUserPlaylistEntryAsync(
                    targetId, playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserPlaylist?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
            _writeRepository.Verify(r => r.AddUserPlaylistEntryAsync(It.IsAny<UserPlaylist>(), It.IsAny<CancellationToken>()), Times.Once);
            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Adds_Entry_When_Requester_Is_Owner_And_Target_Is_Another_User()
        {
            var ownerId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var playlist = BuildPlaylist(isPublic: false, ownerId: ownerId);
            var command = new AddPlaylistToUserCommand(ownerId, targetId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _userReadRepository.Setup(r => r.ExistsAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _writeRepository.Setup(r => r.GetUserPlaylistEntryAsync(
                    targetId, playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserPlaylist?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
            _writeRepository.Verify(r => r.AddUserPlaylistEntryAsync(It.IsAny<UserPlaylist>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Throws_When_Playlist_Missing()
        {
            var playlistId = Guid.NewGuid();
            var command = new AddPlaylistToUserCommand(Guid.NewGuid(), Guid.NewGuid(), playlistId);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlistId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Playlist?)null);

            await Assert.ThrowsAsync<PlaylistNotFoundException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Throws_When_Target_User_Missing()
        {
            var playlist = BuildPlaylist(isPublic: true, ownerId: Guid.NewGuid());
            var targetId = Guid.NewGuid();
            var command = new AddPlaylistToUserCommand(targetId, targetId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _userReadRepository.Setup(r => r.ExistsAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UserNotFoundException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Throws_When_Private_Playlist_And_Requester_Is_Not_Owner()
        {
            var playlist = BuildPlaylist(isPublic: false, ownerId: Guid.NewGuid());
            var requesterId = Guid.NewGuid();
            var command = new AddPlaylistToUserCommand(requesterId, requesterId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _userReadRepository.Setup(r => r.ExistsAsync(requesterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<PrivatePlaylistAccessException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Throws_When_Target_Is_Owner()
        {
            var ownerId = Guid.NewGuid();
            var playlist = BuildPlaylist(isPublic: true, ownerId: ownerId);
            var command = new AddPlaylistToUserCommand(ownerId, ownerId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _userReadRepository.Setup(r => r.ExistsAsync(ownerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<RedundantOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Throws_When_Entry_Already_Exists()
        {
            var ownerId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var playlist = BuildPlaylist(isPublic: true, ownerId: ownerId);
            var existing = new UserPlaylist { UserId = targetId, PlaylistId = playlist.Id };
            var command = new AddPlaylistToUserCommand(targetId, targetId, playlist.Id);

            _writeRepository.Setup(r => r.GetByIdForUpdateAsync(playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(playlist);
            _userReadRepository.Setup(r => r.ExistsAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _writeRepository.Setup(r => r.GetUserPlaylistEntryAsync(
                    targetId, playlist.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            await Assert.ThrowsAsync<RedundantOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            _writeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}