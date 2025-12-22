using Service.Application.Common.Handlers;

namespace TestProject.Application.Handlers;

public interface IGetUsersHandler : IGetHandler<IEnumerable<User>>
{
}