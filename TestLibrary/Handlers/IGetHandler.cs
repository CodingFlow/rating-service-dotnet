namespace TestLibrary.Application.Handlers;

public interface IGetHandler<TResponse>
{
    TResponse Handle();
}
