using Shouldly;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Domain.UnitTests.Entities;

public sealed class ManagementUserTests
{
    [Fact]
    public void Constructor_ShouldCreateUser_WhenValuesAreValid()
    {
        var id = Guid.NewGuid();

        var user = new ManagementUser(id, "username", "user@example.com", "hashed-password");

        user.Id.ShouldBe(id);
        user.Username.ShouldBe("username");
        user.Email.ShouldBe("user@example.com");
        user.PasswordHash.ShouldBe("hashed-password");
    }

    [Fact]
    public void Constructor_ShouldTrimTextValues_WhenValuesContainPadding()
    {
        var user = new ManagementUser(Guid.NewGuid(), "  username  ", "  user@example.com  ", "  hashed-password  ");

        user.Username.ShouldBe("username");
        user.Email.ShouldBe("user@example.com");
        user.PasswordHash.ShouldBe("hashed-password");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenIdIsEmpty()
    {
        Should.Throw<InvalidManagementUserException>(() =>
            new ManagementUser(Guid.Empty, "username", "user@example.com", "hashed-password"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenUsernameIsInvalid(string? username)
    {
        Should.Throw<InvalidManagementUserException>(() =>
            new ManagementUser(Guid.NewGuid(), username!, "user@example.com", "hashed-password"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenEmailIsInvalid(string? email)
    {
        Should.Throw<InvalidManagementUserException>(() =>
            new ManagementUser(Guid.NewGuid(), "username", email!, "hashed-password"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenPasswordHashIsInvalid(string? passwordHash)
    {
        Should.Throw<InvalidManagementUserException>(() =>
            new ManagementUser(Guid.NewGuid(), "username", "user@example.com", passwordHash!));
    }
}
