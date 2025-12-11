namespace Service.Application.Common.Handlers;

public interface IGetHandler<TResponse>
{
    TResponse Handle();
}
