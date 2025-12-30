namespace Service.Application.Common.Handlers;

public interface IHandler<TRequest>
{
    Task Handle(TRequest request);
}
