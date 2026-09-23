using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Billing.Commands;

namespace Zentric.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BillingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("invoices/generate/{orderId}")]
        public async Task<IActionResult> GenerateInvoices(Guid orderId)
        {
            var result = await _mediator.Send(new GenerateInvoicesCommand(orderId));
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }
    }
}
