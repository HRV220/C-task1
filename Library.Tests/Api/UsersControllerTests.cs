using Library.Api.Contracts.Requests;
using Library.Api.Contracts.Responses;
using Library.Api.Controllers;
using Library.Application.Abstractions.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests.Api;

public sealed class UsersControllerTests
{
    private readonly Mock<IUserService> _users = new();

    private UsersController CreateController()
        => new(_users.Object);

    [Fact]
    public void Register_ReturnsCreatedWithMappedUser()
    {
        var user = new User("Ivan", new[] { UserRole.Reader });
        _users.Setup(service => service.Register("Ivan", It.IsAny<IReadOnlyCollection<UserRole>>())).Returns(user);
        var controller = CreateController();

        var result = controller.Register(new RegisterUserRequest("Ivan", new[] { UserRole.Reader }));

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<UserResponse>(created.Value);
        Assert.Equal("Ivan", response.Name);
        Assert.Contains("Reader", response.Roles);
    }

    [Fact]
    public void GetByName_ReturnsOkWithMappedUser()
    {
        var user = new User("Ivan", new[] { UserRole.Reader, UserRole.Librarian });
        _users.Setup(service => service.GetByName("Ivan")).Returns(user);
        var controller = CreateController();

        var result = controller.GetByName("Ivan");

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserResponse>(ok.Value);
        Assert.Equal("Ivan", response.Name);
        Assert.Equal(2, response.Roles.Count);
    }
}
