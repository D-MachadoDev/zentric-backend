using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentric.Application.Returns.Commands;

namespace Zentric.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReturnsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReturnsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestReturn([FromBody] RequestReturnCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok(result.Value);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveReturn(Guid id, [FromBody] ApproveReturnCommand command)
        {
            if (id != command.ReturnRequestId) return BadRequest();
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok();
        }

        [HttpPost("{id}/inspect")]
        public async Task<IActionResult> InspectReturn(Guid id, [FromBody] InspectReturnRequestDto dto)
        {
            var result = await _mediator.Send(new InspectReturnCommand(id, dto.IsGoodCondition));
            if (result.IsFailure) return BadRequest(new ProblemDetails { Detail = result.Error });
            return Ok();
        }
    }

    public record InspectReturnRequestDto(bool IsGoodCondition);
}
