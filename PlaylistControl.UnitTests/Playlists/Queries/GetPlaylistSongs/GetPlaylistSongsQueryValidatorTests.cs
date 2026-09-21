using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Queries.GetPlaylistSongs;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetPlaylistSongs;

public class GetPlaylistSongsQueryValidatorTests
{
    private readonly GetPlaylistSongsQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors()
    {
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRequesterId_HasError()
    {
        var query = new GetPlaylistSongsQuery(Guid.Empty, Guid.NewGuid(), 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.RequesterId);
    }

    [Fact]
    public void Validate_EmptyPlaylistId_HasError()
    {
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), Guid.Empty, 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PlaylistId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageLessThanOne_HasError(int page)
    {
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), Guid.NewGuid(), page, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageSizeLessThanOne_HasError(int pageSize)
    {
        var query = new GetPlaylistSongsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, pageSize);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}