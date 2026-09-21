using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Playlists.Queries.GetUserPlaylists;

namespace PlaylistControl.UnitTests.Playlists.Queries.GetUserPlaylists;

public class GetUserPlaylistsQueryValidatorTests
{
    private readonly GetUserPlaylistsQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors()
    {
        var query = new GetUserPlaylistsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRequesterId_HasError()
    {
        var query = new GetUserPlaylistsQuery(Guid.Empty, Guid.NewGuid(), 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.RequesterId);
    }

    [Fact]
    public void Validate_EmptyUserId_HasError()
    {
        var query = new GetUserPlaylistsQuery(Guid.NewGuid(), Guid.Empty, 1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageLessThanOne_HasError(int page)
    {
        var query = new GetUserPlaylistsQuery(Guid.NewGuid(), Guid.NewGuid(), page, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageSizeLessThanOne_HasError(int pageSize)
    {
        var query = new GetUserPlaylistsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, pageSize);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}