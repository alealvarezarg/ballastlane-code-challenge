using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("management-tasks")]
public sealed class ManagementTaskController : ControllerBase
{
    private readonly IManagementTaskService _managementTaskService;

    public ManagementTaskController(IManagementTaskService managementTaskService)
    {
        _managementTaskService = managementTaskService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedManagementTaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedManagementTaskResponseDto>> GetAllAsync([FromQuery] ManagementTaskQueryRequestDto request)
    {
        var tasks = await _managementTaskService.QueryAsync(request.ToQueryOptions());

        return Ok(tasks.ToResponseDto());
    }

    [HttpGet("{id:guid}", Name = "GetManagementTaskById")]
    [ProducesResponseType(typeof(ManagementTaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ManagementTaskResponseDto>> GetByIdAsync(Guid id)
    {
        var task = await _managementTaskService.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        return Ok(task.ToResponseDto());
    }

    [HttpPost]
    [ProducesResponseType(typeof(ManagementTaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ManagementTaskResponseDto>> CreateAsync(
        CreateManagementTaskRequestDto request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
    {
        var createdTask = await _managementTaskService.CreateAsync(request.ToDomain(), idempotencyKey);
        var response = createdTask.ToResponseDto();

        return CreatedAtRoute("GetManagementTaskById", new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ManagementTaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ManagementTaskResponseDto>> UpdateAsync(Guid id, UpdateManagementTaskRequestDto request)
    {
        EnsureIdentifiersMatch(id, request.Id);

        var updatedTask = await _managementTaskService.UpdateAsync(request.ToDomain());

        return Ok(updatedTask.ToResponseDto());
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        _ = await _managementTaskService.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        await _managementTaskService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ManagementTaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ManagementTaskResponseDto>>> GetOverdueAsync([FromQuery] bool includeArchived = false)
    {
        var tasks = await _managementTaskService.GetOverdueAsync(includeArchived, DateTime.UtcNow);

        return Ok(tasks.Select(task => task.ToResponseDto()).ToArray());
    }

    [HttpGet("due-within")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ManagementTaskResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<ManagementTaskResponseDto>>> GetDueWithinAsync([FromQuery] int days, [FromQuery] bool includeArchived = false)
    {
        if (days <= 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(days), "Days must be greater than zero.")
            });
        }

        var tasks = await _managementTaskService.GetDueWithinAsync(days, includeArchived, DateTime.UtcNow);

        return Ok(tasks.Select(task => task.ToResponseDto()).ToArray());
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ManagementTaskSummaryResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ManagementTaskSummaryResponseDto>> GetSummaryAsync([FromQuery] bool includeArchived = false)
    {
        var summary = await _managementTaskService.GetSummaryAsync(includeArchived);

        return Ok(summary.ToResponseDto());
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(ManagementTaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ManagementTaskResponseDto>> PatchAsync(Guid id, PatchManagementTaskRequestDto request)
    {
        var updatedTask = await _managementTaskService.PatchAsync(id, request.ToPatch());

        return Ok(updatedTask.ToResponseDto());
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ManagementTaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ManagementTaskResponseDto>> UpdateStatusAsync(Guid id, UpdateManagementTaskStatusRequestDto request)
    {
        var updatedTask = await _managementTaskService.UpdateStatusAsync(id, request.Status);

        return Ok(updatedTask.ToResponseDto());
    }

    private static void EnsureIdentifiersMatch(Guid routeId, Guid payloadId)
    {
        if (routeId == payloadId)
        {
            return;
        }

        throw new ValidationException(new[]
        {
            new ValidationFailure(nameof(UpdateManagementTaskRequestDto.Id), "Route identifier and payload identifier must match.")
        });
    }
}
