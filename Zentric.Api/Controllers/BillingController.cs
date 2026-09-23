using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Billing.Commands;

namespace Zentric.Api.Controllers
{
    /// <summary>
    /// Facturación comercial, generación de factura maestra para el comprador y liquidación de comisión por intermediación para Zentric.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("8. Facturación y Liquidación")]
    [Produces("application/json", "application/problem+json")]
    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BillingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Genera las facturas comerciales de una orden y el desglose de comisión interna para Zentric.
        /// </summary>
        /// <remarks>
        /// Emite la Factura Maestra correspondiente al valor total cancelado por el comprador,
        /// genera la factura de comisión por servicios de intermediación tecnológica para Zentric (ZentricDetail)
        /// y calcula la liquidación neta a dispersar a los vendedores participantes.
        /// </remarks>
        /// <param name="orderId">Identificador único (Guid) del pedido pagado a facturar.</param>
        /// <response code="200">Facturación procesada y emitida con éxito. Retorna true.</response>
        /// <response code="400">Error si la orden no existe, no ha sido pagada o ya tiene facturación generada (RFC 7807 ProblemDetails).</response>
        [HttpPost("invoices/generate/{orderId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateInvoices(Guid orderId)
        {
            var result = await _mediator.Send(new GenerateInvoicesCommand(orderId));
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }
    }
}
