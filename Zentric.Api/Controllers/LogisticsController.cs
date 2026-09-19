using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Logistics.Commands;

namespace Zentric.Api.Controllers
{
    public class LogisticsController : ApiControllerBase
    {
        [HttpPost("fulfillment")]
        public async Task<IActionResult> CreateFulfillment([FromBody] CreateFulfillmentRequest request)
        {
            var command = new CreateFulfillmentOrderCommand(request.CustomerOrderId, request.VendorId);
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }

    public record CreateFulfillmentRequest(Guid CustomerOrderId, Guid VendorId);
}
