using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Queries.GetMyPlaylists;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetMyPlaylists;

public class GetMyPlaylistsQueryValidatorTests
{
    private readonly GetMyPlaylistsQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors()
    {
        var query = new GetMyPlaylistsQuery(Guid.NewGuid(), 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRequesterId_HasError()
    {
        var query = new GetMyPlaylistsQuery(Guid.Empty, 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.RequesterId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageLessThanOne_HasError(int page)
    {
        var query = new GetMyPlaylistsQuery(Guid.NewGuid(), page, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageSizeLessThanOne_HasError(int pageSize)
    {
        var query = new GetMyPlaylistsQuery(Guid.NewGuid(), 1, pageSize);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}