namespace Service.Application.Common.Handlers;

public interface IPostHandler<TRequest>
{
    Task Handle(TRequest request);
}
