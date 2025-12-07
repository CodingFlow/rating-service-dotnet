namespace RatingService.Application.Handlers;

public interface IPostHandler<TRequest, TResponse>
{
    TResponse Handle(TRequest request);
}
