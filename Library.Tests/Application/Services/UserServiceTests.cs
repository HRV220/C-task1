using Library.Application.Abstractions.Repositories;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;
using Moq;
using Xunit;

namespace Library.Tests.Application.Services;

public sealed class UserServiceTests
{
    private readonly Mock<IUserRepository> _users = new();

    private UserService CreateService()
        => new(_users.Object);

    [Fact]
    public void Register_WithUniqueName_AddsAndReturnsUser()
    {
        _users.Setup(r => r.FindByName("Ivan")).Returns((User?)null);
        _users.Setup(r => r.Add(It.IsAny<User>())).Returns<User>(user => user);
        var service = CreateService();

        var user = service.Register("Ivan", new[] { UserRole.Reader });

        Assert.Equal("Ivan", user.Name);
        _users.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public void Register_WithDuplicateName_ThrowsDuplicateUserException()
    {
        var existing = new User("Ivan", new[] { UserRole.Reader });
        _users.Setup(r => r.FindByName("Ivan")).Returns(existing);
        var service = CreateService();

        Assert.Throws<DuplicateUserException>(() => service.Register("Ivan", new[] { UserRole.Writer }));
        _users.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void GetByName_WhenUserExists_ReturnsUser()
    {
        var existing = new User("Ivan", new[] { UserRole.Reader });
        _users.Setup(r => r.FindByName("Ivan")).Returns(existing);
        var service = CreateService();

        var user = service.GetByName("Ivan");

        Assert.Same(existing, user);
    }

    [Fact]
    public void GetByName_WhenUserMissing_ThrowsUserNotFoundException()
    {
        _users.Setup(r => r.FindByName("Ghost")).Returns((User?)null);
        var service = CreateService();

        Assert.Throws<UserNotFoundException>(() => service.GetByName("Ghost"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GetByName_WithEmptyName_ThrowsArgumentException(string name)
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.GetByName(name));
    }
}
