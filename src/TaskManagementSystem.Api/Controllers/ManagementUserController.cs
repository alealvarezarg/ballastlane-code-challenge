using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("management-users")]
public sealed class ManagementUserController : ControllerBase
{
    private readonly IManagementUserService _managementUserService;

    public ManagementUserController(IManagementUserService managementUserService)
    {
        _managementUserService = managementUserService;
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(ManagementUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ManagementUserResponseDto>> CreateAsync(CreateManagementUserRequestDto request)
    {
        var createdUser = await _managementUserService.CreateAsync(request.Username, request.Email, request.Password);
        var response = createdUser.ToResponseDto();

        return CreatedAtRoute("GetManagementUserById", new { id = response.Id }, response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ManagementUserLoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ManagementUserLoginResponseDto>> LoginAsync(LoginManagementUserRequestDto request)
    {
        var authenticatedUser = await _managementUserService.LoginAsync(request.Email, request.Password);

        return Ok(authenticatedUser.ToResponseDto());
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}", Name = "GetManagementUserById")]
    [ProducesResponseType(typeof(ManagementUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ManagementUserResponseDto>> GetByIdAsync(Guid id)
    {
        var user = await _managementUserService.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementUser with id '{id}' was not found.");

        return Ok(user.ToResponseDto());
    }
}
