using MediatR;
using Microsoft.AspNetCore.Mvc;
using Withly.Application.Auth.Commands;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Exceptions;
using FluentValidation;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        try
        {
            var token = await mediator.Send(command);
            return Created(string.Empty, new AuthResultDto { Token = token });
        }
        catch (UserAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return BadRequest(new ValidationProblemDetails(errors));
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var token = await mediator.Send(command);
        return Ok(new { token });
    }
}