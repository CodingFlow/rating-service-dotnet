using Service.Application.Common.Handlers;

namespace TestProject.Application.Handlers;

public interface IPostUsersHandler : IPostHandler<User, string>
{
}