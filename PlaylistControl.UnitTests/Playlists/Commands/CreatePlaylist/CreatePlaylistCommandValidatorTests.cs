using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Commands.CreatePlaylist;

namespace PlaylistControl.UnitTests.Playlists.Commands.CreatePlaylist;

public class CreatePlaylistCommandValidatorTests
{
    private readonly CreatePlaylistCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new CreatePlaylistCommand(Guid.NewGuid(), "Road Trip", true);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRequesterId_HasError()
    {
        var command = new CreatePlaylistCommand(Guid.Empty, "Road Trip", true);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequesterId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyOrWhitespaceName_HasError(string name)
    {
        var command = new CreatePlaylistCommand(Guid.NewGuid(), name, true);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameOver200Chars_HasError()
    {
        var command = new CreatePlaylistCommand(Guid.NewGuid(), new string('a', 201), true);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameAt200Chars_HasNoErrors()
    {
        var command = new CreatePlaylistCommand(Guid.NewGuid(), new string('a', 200), true);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}