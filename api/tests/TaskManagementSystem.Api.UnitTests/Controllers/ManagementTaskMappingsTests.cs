using Shouldly;
using System.Text.Json;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementTaskMappingsTests
{
    [Fact]
    public void CreateRequest_ShouldMapToDomainTask()
    {
        var request = new CreateManagementTaskRequestDto
        {
            Title = "Task title",
            Description = "Task description",
            Status = ManagementTaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var result = request.ToDomain();

        result.Id.ShouldNotBe(Guid.Empty);
        result.Title.ShouldBe(request.Title);
        result.Description.ShouldBe(request.Description);
        result.Status.ShouldBe(request.Status!.Value);
        result.DueDate.ShouldBe(request.DueDate);
        result.UserId.ShouldBe(request.UserId);
    }

    [Fact]
    public void CreateRequest_ShouldDefaultStatusToPending_WhenStatusIsOmitted()
    {
        var request = new CreateManagementTaskRequestDto
        {
            Title = "Task title",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var result = request.ToDomain();

        result.Status.ShouldBe(ManagementTaskStatus.Pending);
    }

    [Fact]
    public void UpdateRequest_ShouldMapToDomainTask()
    {
        var request = new UpdateManagementTaskRequestDto
        {
            Id = Guid.NewGuid(),
            Title = "Task title",
            Description = "Task description",
            Status = ManagementTaskStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(2),
            UserId = Guid.NewGuid()
        };

        var result = request.ToDomain();

        result.Id.ShouldBe(request.Id);
        result.Title.ShouldBe(request.Title);
        result.Description.ShouldBe(request.Description);
        result.Status.ShouldBe(request.Status);
        result.DueDate.ShouldBe(request.DueDate);
        result.UserId.ShouldBe(request.UserId);
    }

    [Fact]
    public void DomainTask_ShouldMapToResponseDto()
    {
        var task = new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.Completed,
            DateTime.UtcNow.AddDays(3),
            Guid.NewGuid());

        var result = task.ToResponseDto();

        result.Id.ShouldBe(task.Id);
        result.Title.ShouldBe(task.Title);
        result.Description.ShouldBe(task.Description);
        result.Status.ShouldBe(task.Status);
        result.DueDate.ShouldBe(task.DueDate);
        result.UserId.ShouldBe(task.UserId);
        result.IsArchived.ShouldBe(task.IsArchived);
    }

    [Fact]
    public void DomainUser_ShouldMapToResponseDto()
    {
        var user = new ManagementUser(Guid.NewGuid(), "manager", "manager@example.com", "hashed-password");

        var result = user.ToResponseDto();

        result.Id.ShouldBe(user.Id);
        result.Username.ShouldBe(user.Username);
        result.Email.ShouldBe(user.Email);
    }

    [Fact]
    public void AuthenticatedUser_ShouldMapToLoginResponseDto()
    {
        var authenticatedUser = new AuthenticatedManagementUser
        {
            Id = Guid.NewGuid(),
            Username = "manager",
            Email = "manager@example.com",
            AccessToken = "access-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        var result = authenticatedUser.ToResponseDto();

        result.AccessToken.ShouldBe(authenticatedUser.AccessToken);
        result.User.Username.ShouldBe(authenticatedUser.Username);
        result.User.Email.ShouldBe(authenticatedUser.Email);
    }

    [Fact]
    public void PagedResult_ShouldMapItemsToResponseDtos()
    {
        var pagedResult = new PagedResult<ManagementTask>
        {
            Items = new[]
            {
                new ManagementTask(
                    Guid.NewGuid(),
                    "Task one",
                    "Description one",
                    ManagementTaskStatus.Pending,
                    DateTime.UtcNow.AddDays(1),
                    Guid.NewGuid()),
                new ManagementTask(
                    Guid.NewGuid(),
                    "Task two",
                    "Description two",
                    ManagementTaskStatus.InProgress,
                    DateTime.UtcNow.AddDays(2),
                    Guid.NewGuid())
            },
            Page = 2,
            PageSize = 10,
            TotalCount = 12
        };

        var result = pagedResult.ToResponseDto();

        result.ShouldBeOfType<PagedManagementTaskResponseDto>();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldAllBe(item => item is ManagementTaskResponseDto);
        result.Page.ShouldBe(2);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(12);
    }

    [Fact]
    public void Summary_ShouldMapStatusesToCountDtos()
    {
        var summary = new[]
        {
            new ManagementTaskStatusSummary
            {
                Status = ManagementTaskStatus.Pending,
                Count = 3
            },
            new ManagementTaskStatusSummary
            {
                Status = ManagementTaskStatus.Completed,
                Count = 4
            }
        };

        var result = summary.ToResponseDto();

        result.ShouldBeOfType<ManagementTaskSummaryResponseDto>();
        result.Statuses.Count.ShouldBe(2);
        result.Statuses.ShouldAllBe(item => item is ManagementTaskStatusCountDto);
        result.Statuses.Select(item => item.Status).ShouldBe(new[]
        {
            ManagementTaskStatus.Pending,
            ManagementTaskStatus.Completed
        });
    }

    [Fact]
    public void ResponseDto_ShouldSerializeStatusAsReadableString()
    {
        var task = new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.InProgress,
            DateTime.UtcNow.AddDays(1),
            Guid.NewGuid());

        var json = JsonSerializer.Serialize(task.ToResponseDto(), CreateApiJsonOptions());

        json.ShouldContain("\"status\":\"InProgress\"");
        json.ShouldNotContain("\"status\":1");
    }

    [Fact]
    public void SummaryDto_ShouldSerializeNestedStatusesAsReadableStrings()
    {
        var summary = new[]
        {
            new ManagementTaskStatusSummary
            {
                Status = ManagementTaskStatus.Pending,
                Count = 3
            }
        };

        var json = JsonSerializer.Serialize(summary.ToResponseDto(), CreateApiJsonOptions());

        json.ShouldContain("\"status\":\"Pending\"");
        json.ShouldNotContain("\"status\":0");
    }

    private static JsonSerializerOptions CreateApiJsonOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(allowIntegerValues: false));
        return options;
    }
}
