using Shouldly;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Domain.UnitTests.Entities;

public sealed class ManagementTaskEnhancementTests
{
    [Fact]
    public void CreateNew_ShouldDefaultStatusToPending_WhenStatusIsOmitted()
    {
        var task = ManagementTask.CreateNew(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            null,
            DateTime.UtcNow.AddDays(1),
            Guid.NewGuid());

        task.Status.ShouldBe(ManagementTaskStatus.Pending);
    }

    [Fact]
    public void ChangeStatus_ShouldThrowConflict_WhenTransitionIsNotAllowed()
    {
        var task = CreateCompletedTask();

        Should.Throw<ManagementTaskLifecycleException>(() => task.ChangeStatus(ManagementTaskStatus.Pending));
    }

    [Fact]
    public void UpdateInformation_ShouldThrowConflict_WhenTaskIsCompleted()
    {
        var task = CreateCompletedTask();

        Should.Throw<ManagementTaskLifecycleException>(() =>
            task.UpdateInformation("Updated", "Updated", DateTime.UtcNow.AddDays(5), Guid.NewGuid()));
    }

    [Fact]
    public void Archive_ShouldSetArchiveMetadata_WhenTaskIsEligible()
    {
        var task = CreatePendingTask();
        var archivedAt = DateTime.UtcNow;

        task.Archive(archivedAt);

        task.IsArchived.ShouldBeTrue();
        task.ArchivedAt.ShouldBe(archivedAt);
    }

    private static ManagementTask CreatePendingTask()
    {
        return new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.Pending,
            DateTime.UtcNow.AddDays(2),
            Guid.NewGuid());
    }

    private static ManagementTask CreateCompletedTask()
    {
        return new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.Completed,
            DateTime.UtcNow.AddDays(2),
            Guid.NewGuid());
    }
}
