using FluentValidation;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public class ValidationRequestDecorator<TRequest, TResponse>(IResponseStrategy<TRequest, TResponse> responseStrategy, IValidator<TRequest> validator) : IResponseStrategy<TRequest, TResponse>
{
    public async Task<Result<Response<TResponse>>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler)
    {
        var validationResult = validator.Validate(request);

        if (validationResult.IsValid)
        {
            return await responseStrategy.CreateResponse(request, handler);
        } 
        else
        {
            var validationError = validationResult.Errors.First();

            return new Result<Response<TResponse>>
            {
                Error = new Response<ValidationError>
                {
                    StatusCode = 422,
                    Headers = [],
                    Body = new ValidationError()
                    {
                        ErrorCode = ErrorCode.InvalidInput,
                        Message = validationError.ErrorMessage,
                        Location = validationError.PropertyName
                    }
                }
            };
        }

    }
}
