using AutoFixture;
using FluentAssertions;
using Moq;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Application.Features.Users.Queries.GetAllUsers;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.UnitTests.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IUserReadRepository> _userReadRepository = new();
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _handler = new GetAllUsersQueryHandler(_userReadRepository.Object);
    }

    private User BuildUser() =>
        _fixture.Build<User>()
            .Without(u => u.UserPlaylists)
            .Create();

    [Fact]
    public async Task Handle_EmptyPage_ReturnsEmptyItemsWithMetadata()
    {
        var query = new GetAllUsersQuery(1, 1);
        _userReadRepository
            .Setup(r => r.GetAllAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<User>(Array.Empty<User>(), 1, 1, 0));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_FirstPage_MapsItemsAndMetadata()
    {
        var user = BuildUser();
        var query = new GetAllUsersQuery(1, 1);
        _userReadRepository
            .Setup(r => r.GetAllAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<User>(new[] { user }, 1, 1, 3));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.Single().Id.Should().Be(user.Id);
        result.Items.Single().Username.Should().Be(user.Username);
        result.Items.Single().Email.Should().Be(user.Email);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_PageBeyondRange_ReturnsEmptyItemsWithMetadata()
    {
        var query = new GetAllUsersQuery(10, 1);
        _userReadRepository
            .Setup(r => r.GetAllAsync(10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<User>(Array.Empty<User>(), 10, 1, 3));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Page.Should().Be(10);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }
}