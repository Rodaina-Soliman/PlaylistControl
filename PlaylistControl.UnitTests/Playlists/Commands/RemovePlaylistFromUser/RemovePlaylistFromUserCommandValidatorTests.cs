using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Commands.RemovePlaylistFromUser;

namespace PlaylistControl.UnitTests.Playlists.Commands.RemovePlaylistFromUser
{
    public class RemovePlaylistFromUserCommandValidatorTests
    {
        private readonly RemovePlaylistFromUserCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_RequesterId_Is_Empty()
        {
            var command = new RemovePlaylistFromUserCommand(Guid.Empty, Guid.NewGuid());
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.RequesterId);
        }

        [Fact]
        public void Should_Have_Error_When_PlaylistId_Is_Empty()
        {
            var command = new RemovePlaylistFromUserCommand(Guid.NewGuid(), Guid.Empty);
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.PlaylistId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Identifiers_Are_Present()
        {
            var command = new RemovePlaylistFromUserCommand(Guid.NewGuid(), Guid.NewGuid());
            _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
        }
    }
}