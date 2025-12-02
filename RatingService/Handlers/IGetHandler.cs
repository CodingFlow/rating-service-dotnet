namespace RatingService.Handlers;

internal interface IGetHandler<TResponse>
{
    TResponse Handle();
}
