namespace Service.Application.Common.Handlers;

public interface IDeleteHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request);
}

