using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Orders.Commands;

namespace Zentric.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("cart")]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }

        [HttpPost("cart/items")]
        public async Task<IActionResult> AddOrderItem([FromBody] AddOrderItemCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok();
        }

        [HttpPost("{orderId}/checkout")]
        public async Task<IActionResult> Checkout(Guid orderId)
        {
            var result = await _mediator.Send(new CheckoutOrderCommand(orderId));
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok();
        }

        [HttpPost("{orderId}/pay")]
        public async Task<IActionResult> Pay(Guid orderId)
        {
            var result = await _mediator.Send(new PayOrderCommand(orderId));
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok();
        }
    }
}
