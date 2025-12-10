using Service.Application.Common.Handlers;

namespace RatingService.Application.Handlers;

public interface IGetUsersHandler : IGetHandler<User[]>
{
}