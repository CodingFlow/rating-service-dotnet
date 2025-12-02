namespace RatingService.Handlers;

internal interface IPostHandler<TRequest, TResponse>
{
    TResponse Handle(TRequest request);
}
