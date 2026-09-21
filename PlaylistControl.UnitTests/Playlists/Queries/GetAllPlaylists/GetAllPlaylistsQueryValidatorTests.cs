using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Queries.GetAllPlaylists;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetAllPlaylists;

public class GetAllPlaylistsQueryValidatorTests
{
    private readonly GetAllPlaylistsQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors()
    {
        var query = new GetAllPlaylistsQuery(Guid.NewGuid(), 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRequesterId_HasError()
    {
        var query = new GetAllPlaylistsQuery(Guid.Empty, 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.RequesterId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageLessThanOne_HasError(int page)
    {
        var query = new GetAllPlaylistsQuery(Guid.NewGuid(), page, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageSizeLessThanOne_HasError(int pageSize)
    {
        var query = new GetAllPlaylistsQuery(Guid.NewGuid(), 1, pageSize);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}