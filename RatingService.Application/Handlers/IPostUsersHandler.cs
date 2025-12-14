using Service.Application.Common.Handlers;

namespace RatingService.Application.Handlers;

public interface IPostUsersHandler : IPostHandler<User[], string>
{
}