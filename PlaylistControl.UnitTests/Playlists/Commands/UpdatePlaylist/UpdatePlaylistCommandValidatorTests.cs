using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Commands.UpdatePlaylist;

namespace PlaylistControl.UnitTests.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandValidatorTests
{
    private readonly UpdatePlaylistCommandValidator _validator = new();

    [Fact]
    public void Validate_BothFieldsNull_HasErrorOnCommand()
    {
        var command = new UpdatePlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrors();
    }

    [Fact]
    public void Validate_OnlyNameProvided_HasNoErrors()
    {
        var command = new UpdatePlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), "New Name", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OnlyIsPublicProvided_HasNoErrors()
    {
        var command = new UpdatePlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), null, false);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyOrWhitespaceName_HasError(string name)
    {
        var command = new UpdatePlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), name, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameOver200Chars_HasError()
    {
        var command = new UpdatePlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), new string('a', 201), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_EmptyRequesterId_HasError()
    {
        var command = new UpdatePlaylistCommand(Guid.Empty, Guid.NewGuid(), "Name", null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequesterId);
    }

    [Fact]
    public void Validate_EmptyPlaylistId_HasError()
    {
        var command = new UpdatePlaylistCommand(Guid.NewGuid(), Guid.Empty, "Name", null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PlaylistId);
    }
}