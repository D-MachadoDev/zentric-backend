using MediatR;
using Zentric.Application.Common.Models;
using Zentric.Domain.Returns.Ports;

namespace Zentric.Application.Returns.Commands
{
    public record InspectReturnCommand(Guid ReturnRequestId, bool IsGoodCondition) : IRequest<Result<bool>>;

    public class InspectReturnCommandHandler : IRequestHandler<InspectReturnCommand, Result<bool>>
    {
        private readonly IReturnRequestRepository _repository;

        public InspectReturnCommandHandler(IReturnRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(InspectReturnCommand request, CancellationToken cancellationToken)
        {
            var returnReq = await _repository.GetByIdAsync(request.ReturnRequestId, cancellationToken);
            if (returnReq == null)
            {
                return Result<bool>.Failure("Return request not found.");
            }

            try
            {
                returnReq.InspectByLogistics(request.IsGoodCondition);
                await _repository.UpdateAsync(returnReq, cancellationToken);
                return Result<bool>.Success(true);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
