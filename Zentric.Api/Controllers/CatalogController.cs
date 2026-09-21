using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Catalog.Commands;

namespace Zentric.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }

        [HttpPost("products/{productId}/publish")]
        public async Task<IActionResult> PublishProduct(Guid productId)
        {
            var result = await _mediator.Send(new PublishProductCommand(productId));
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok();
        }
    }
}
