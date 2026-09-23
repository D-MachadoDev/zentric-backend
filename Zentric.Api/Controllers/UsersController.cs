using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Users.Commands;

namespace Zentric.Api.Controllers
{
    /// <summary>
    /// Gestión de usuarios, participantes del marketplace y asignación de roles operativos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("1. Usuarios y Roles")]
    [Produces("application/json", "application/problem+json")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registra un nuevo usuario en la plataforma Zentric.
        /// </summary>
        /// <remarks>
        /// Valida de forma asíncrona la unicidad del correo electrónico y del documento de identificación fiscal/nacional (IdentityDocument).
        /// Asigna el rol inicial del participante (Buyer, Vendor, Admin, LogisticsOperator, Supervisor) según las reglas de negocio de ZENTRIC.md §5.
        /// </remarks>
        /// <param name="command">Datos de creación del usuario (nombre, correo, rol y documento de identidad).</param>
        /// <response code="200">Usuario registrado con éxito. Retorna el identificador único (Guid) generado.</response>
        /// <response code="400">Error de validación o conflicto por correo/documento duplicado (RFC 7807 ProblemDetails).</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }
    }
}
