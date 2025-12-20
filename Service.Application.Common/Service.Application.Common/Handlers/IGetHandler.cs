namespace Service.Application.Common.Handlers;

public interface IGetHandler<TResponse>
{
    Task<TResponse> Handle();
}
