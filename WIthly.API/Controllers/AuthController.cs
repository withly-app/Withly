using Microsoft.AspNetCore.Mvc;
using Withly.Application.Auth.Dtos;
using Withly.Application.Services;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserService userService, RefreshTokenService refreshTokenService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto, CancellationToken ct = default)
    {
        var result = await userService.RegisterAsync(dto, ct);

        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        // Interpret known errors
        return result.Error switch
        {
            "User already exists" => Conflict(result.Error),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto, CancellationToken ct = default)
    {
        var result = await userService.LoginAsync(dto.Email, dto.Password, ct);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error switch
        {
            "Invalid credentials" => Unauthorized(result.Error),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto, CancellationToken ct = default)
    {
        var result = await refreshTokenService.GetAuthTokenAsync(dto.Token, ct);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestPasswordResetDto dto, CancellationToken ct = default)
    {
        await userService.RequestPasswordResetAsync(dto.Email, ct);
        return Ok(new
        {
            message =
                "You have received a mail to reset your password if we found an account associated with your email."
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto, CancellationToken ct = default)
    {
        var result = await userService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest("Invalid or expired password reset request");
    }
}