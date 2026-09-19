using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Orders.Commands;

namespace Zentric.Api.Controllers
{
    public class OrdersController : ApiControllerBase
    {
        [HttpPost("cart")]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartRequest request)
        {
            var command = new CreateCartCommand(request.BuyerId);
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] AddItemRequest request)
        {
            var command = new AddOrderItemCommand(
                id, 
                request.VariantId, 
                request.Quantity, 
                request.UnitPrice, 
                request.Currency);

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }

    public record CreateCartRequest(Guid BuyerId);
    public record AddItemRequest(Guid VariantId, int Quantity, decimal UnitPrice, string Currency);
}
