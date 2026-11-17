using Microsoft.AspNetCore.Mvc;
using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Features.Auth;

namespace SaludCitas.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IQueryHandler<LoginQuery, AuthResponse> loginHandler,
    ICommandHandler<RegistrarPacienteCommand, AuthResponse> registrarHandler) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginQuery query, CancellationToken cancellationToken)
        => Ok(await loginHandler.Handle(query, cancellationToken));

    [HttpPost("registro")]
    public async Task<ActionResult<AuthResponse>> Registrar(
        RegistrarPacienteCommand command, CancellationToken cancellationToken)
    {
        var respuesta = await registrarHandler.Handle(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, respuesta);
    }
}