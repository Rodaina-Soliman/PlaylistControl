using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Commands.AddPlaylistToUser;

namespace PlaylistControl.UnitTests.Playlists.Commands.AddPlaylistToUser
{
    public class AddPlaylistToUserCommandValidatorTests
    {
        private readonly AddPlaylistToUserCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_RequesterId_Is_Empty()
        {
            var command = new AddPlaylistToUserCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.RequesterId);
        }

        [Fact]
        public void Should_Have_Error_When_TargetUserId_Is_Empty()
        {
            var command = new AddPlaylistToUserCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.TargetUserId);
        }

        [Fact]
        public void Should_Have_Error_When_PlaylistId_Is_Empty()
        {
            var command = new AddPlaylistToUserCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.PlaylistId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Identifiers_Are_Present()
        {
            var command = new AddPlaylistToUserCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
        }
    }
}