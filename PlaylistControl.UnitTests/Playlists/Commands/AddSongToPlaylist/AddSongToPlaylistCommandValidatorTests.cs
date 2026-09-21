using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Commands.AddSongToPlaylist;

namespace PlaylistControl.UnitTests.Playlists.Commands.AddSongToPlaylist;

public class AddSongToPlaylistCommandValidatorTests
{
    private readonly AddSongToPlaylistCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new AddSongToPlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRequesterId_HasError()
    {
        var command = new AddSongToPlaylistCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequesterId);
    }

    [Fact]
    public void Validate_EmptyPlaylistId_HasError()
    {
        var command = new AddSongToPlaylistCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PlaylistId);
    }

    [Fact]
    public void Validate_EmptySongId_HasError()
    {
        var command = new AddSongToPlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SongId);
    }
}