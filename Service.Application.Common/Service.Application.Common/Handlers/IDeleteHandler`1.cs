namespace Service.Application.Common.Handlers;

public interface IDeleteHandler<TRequest>
{
    Task Handle(TRequest request);
}

