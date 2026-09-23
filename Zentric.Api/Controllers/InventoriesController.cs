using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Inventories.Commands;

namespace Zentric.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("stock")]
        public async Task<IActionResult> AddStock([FromBody] AddStockCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }
    }
}
