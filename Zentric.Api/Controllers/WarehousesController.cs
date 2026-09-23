using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Warehouses.Commands;

namespace Zentric.Api.Controllers
{
    /// <summary>
    /// Administración de bodegas físicas, capacidad volumétrica y centros de acopio.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("2. Bodegas")]
    [Produces("application/json", "application/problem+json")]
    public class WarehousesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WarehousesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registra y da de alta una nueva bodega física de almacenamiento.
        /// </summary>
        /// <remarks>
        /// Registra una nueva infraestructura de almacenamiento físico para vendedores o la red logística de Zentric,
        /// definiendo nombre, dirección física y capacidad volumétrica máxima en metros cúbicos (m³).
        /// </remarks>
        /// <param name="command">Datos de registro de la bodega (nombre, ubicación y capacidad volumétrica).</param>
        /// <response code="200">Bodega registrada con éxito. Retorna el identificador único (Guid) generado.</response>
        /// <response code="400">Error de validación si el nombre está vacío o la capacidad volumétrica es menor o igual a cero (RFC 7807 ProblemDetails).</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }
    }
}
