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
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ManagementUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ManagementUserResponseDto>>> GetAllAsync()
    {
        var users = await _managementUserService.GetAllAsync();

        return Ok(users.Select(user => user.ToResponseDto()).ToArray());
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

        return StatusCode(StatusCodes.Status201Created, response);
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

}
