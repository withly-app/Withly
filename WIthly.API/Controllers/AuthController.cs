using MediatR;
using Microsoft.AspNetCore.Mvc;
using Withly.Application.Auth.Commands;
using Withly.Application.Auth.Dtos;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await mediator.Send(command);

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
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await mediator.Send(command);
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
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> ResetPassword([FromBody] RequestPasswordResetCommand command)
    {
        await mediator.Send(command);
        return Ok(new
        {
            message =
                "You have received a mail to reset your password if we found an account associated with your email."
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error switch
        {
            "User not found" => Problem("Invalid or expired password reset request"),
            _ => BadRequest(new { error = result.Error })
        };
    }
}