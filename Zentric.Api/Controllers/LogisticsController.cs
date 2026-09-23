using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Logistics.Commands;

namespace Zentric.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogisticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LogisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("fulfillment")]
        public async Task<IActionResult> CreateFulfillment([FromBody] CreateFulfillmentOrderCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }

        [HttpPost("fulfillment/{id}/dispatch")]
        public async Task<IActionResult> DispatchFulfillment(Guid id)
        {
            var result = await _mediator.Send(new DispatchFulfillmentCommand(id));
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok();
        }

        [HttpPost("fulfillment/cancel-ghost-stock")]
        public async Task<IActionResult> CancelGhostStock([FromBody] CancelFulfillmentOrderDueToNoStockCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }
    }
}
