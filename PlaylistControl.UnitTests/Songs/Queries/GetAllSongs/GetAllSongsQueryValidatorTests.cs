using FluentValidation.TestHelper;
using PlaylistControl.Application.Features.Songs.Queries.GetAllSongs;

namespace PlaylistControl.UnitTests.Songs.Queries.GetAllSongs;

public class GetAllSongsQueryValidatorTests
{
    private readonly GetAllSongsQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors()
    {
        var query = new GetAllSongsQuery(1, 1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageLessThanOne_HasError(int page)
    {
        var query = new GetAllSongsQuery(page, 1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageSizeLessThanOne_HasError(int pageSize)
    {
        var query = new GetAllSongsQuery(1, pageSize);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}