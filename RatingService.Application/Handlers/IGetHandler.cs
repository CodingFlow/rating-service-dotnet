namespace RatingService.Application.Handlers;

public interface IGetHandler<TResponse>
{
    TResponse Handle();
}
