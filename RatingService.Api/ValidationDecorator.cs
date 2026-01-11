using FluentValidation;
using RequestDecoratorGenerator;
using Service.Api.Common;
using Service.Api.Common.ResponseStrategies;

namespace RatingService.Api;

[RequestDecorator]
internal class ValidationDecorator<TRequest, TResponse>(IResponseStrategy<TRequest, TResponse> responseStrategy, IValidator<TRequest> validator) : ValidationRequestDecorator<TRequest, TResponse>(responseStrategy, validator)
{
}
