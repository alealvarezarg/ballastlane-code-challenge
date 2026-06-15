using Shouldly;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Domain.UnitTests.Entities;

public sealed class ManagementTaskTests
{
    [Fact]
    public void Constructor_ShouldCreateTask_WhenValuesAreValid()
    {
        var id = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(5);
        var userId = Guid.NewGuid();

        var task = new ManagementTask(id, "Task title", "Task description", ManagementTaskStatus.Pending, dueDate, userId);

        task.Id.ShouldBe(id);
        task.Title.ShouldBe("Task title");
        task.Description.ShouldBe("Task description");
        task.Status.ShouldBe(ManagementTaskStatus.Pending);
        task.DueDate.ShouldBe(dueDate);
        task.UserId.ShouldBe(userId);
    }

    [Fact]
    public void Constructor_ShouldTrimTextValues_WhenValuesContainPadding()
    {
        var task = new ManagementTask(
            Guid.NewGuid(),
            "  Task title  ",
            "  Task description  ",
            ManagementTaskStatus.Pending,
            DateTime.UtcNow.AddDays(1),
            Guid.NewGuid());

        task.Title.ShouldBe("Task title");
        task.Description.ShouldBe("Task description");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenIdIsEmpty()
    {
        Should.Throw<InvalidManagementTaskException>(() =>
            new ManagementTask(Guid.Empty, "Task title", "Task description", ManagementTaskStatus.Pending, DateTime.UtcNow.AddDays(1), Guid.NewGuid()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenTitleIsInvalid(string? title)
    {
        Should.Throw<InvalidManagementTaskException>(() =>
            new ManagementTask(Guid.NewGuid(), title!, "Task description", ManagementTaskStatus.Pending, DateTime.UtcNow.AddDays(1), Guid.NewGuid()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenDescriptionIsInvalid(string? description)
    {
        Should.Throw<InvalidManagementTaskException>(() =>
            new ManagementTask(Guid.NewGuid(), "Task title", description!, ManagementTaskStatus.Pending, DateTime.UtcNow.AddDays(1), Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDueDateIsDefault()
    {
        Should.Throw<InvalidManagementTaskException>(() =>
            new ManagementTask(Guid.NewGuid(), "Task title", "Task description", ManagementTaskStatus.Pending, default, Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUserIdIsEmpty()
    {
        Should.Throw<InvalidManagementTaskException>(() =>
            new ManagementTask(Guid.NewGuid(), "Task title", "Task description", ManagementTaskStatus.Pending, DateTime.UtcNow.AddDays(1), Guid.Empty));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateFields_WhenValuesAreValid()
    {
        var task = CreateTask();
        var updatedDueDate = DateTime.UtcNow.AddDays(7);
        var updatedUserId = Guid.NewGuid();

        task.UpdateInformation("Updated title", "Updated description", updatedDueDate, updatedUserId);

        task.Title.ShouldBe("Updated title");
        task.Description.ShouldBe("Updated description");
        task.DueDate.ShouldBe(updatedDueDate);
        task.UserId.ShouldBe(updatedUserId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInformation_ShouldThrow_WhenTitleIsInvalid(string? title)
    {
        var task = CreateTask();

        Should.Throw<InvalidManagementTaskException>(() =>
            task.UpdateInformation(title!, "Updated description", DateTime.UtcNow.AddDays(7), Guid.NewGuid()));
    }

    [Fact]
    public void UpdateInformation_ShouldThrow_WhenDueDateIsDefault()
    {
        var task = CreateTask();

        Should.Throw<InvalidManagementTaskException>(() =>
            task.UpdateInformation("Updated title", "Updated description", default, Guid.NewGuid()));
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatus_WhenStatusIsValid()
    {
        var task = CreateTask();

        task.ChangeStatus(ManagementTaskStatus.Completed);

        task.Status.ShouldBe(ManagementTaskStatus.Completed);
    }

    [Fact]
    public void ChangeStatus_ShouldThrow_WhenStatusIsInvalid()
    {
        var task = CreateTask();

        Should.Throw<InvalidManagementTaskException>(() => task.ChangeStatus((ManagementTaskStatus)999));
    }

    private static ManagementTask CreateTask()
    {
        return new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.Pending,
            DateTime.UtcNow.AddDays(2),
            Guid.NewGuid());
    }
}
