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
    [ProducesResponseType(typeof(IReadOnlyCollection<ManagementTaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ManagementTaskResponseDto>>> GetAllAsync()
    {
        var tasks = await _managementTaskService.GetAllAsync();

        return Ok(tasks.Select(task => task.ToResponseDto()).ToArray());
    }

    [HttpGet("{id:guid}")]
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
    public async Task<ActionResult<ManagementTaskResponseDto>> CreateAsync(CreateManagementTaskRequestDto request)
    {
        var createdTask = await _managementTaskService.CreateAsync(request.ToDomain());
        var response = createdTask.ToResponseDto();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ManagementTaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ManagementTaskResponseDto>> UpdateAsync(Guid id, UpdateManagementTaskRequestDto request)
    {
        EnsureIdentifiersMatch(id, request.Id);

        var updatedTask = await _managementTaskService.UpdateAsync(request.ToDomain());

        return Ok(updatedTask.ToResponseDto());
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        _ = await _managementTaskService.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        await _managementTaskService.DeleteAsync(id);

        return NoContent();
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
