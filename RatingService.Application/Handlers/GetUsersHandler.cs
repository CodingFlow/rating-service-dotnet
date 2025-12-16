using RatingService.Application.Models;
using RatingService.Application.QueryResponses;

namespace RatingService.Application.Handlers;

internal class GetUsersHandler : IGetUsersHandler
{
    public GetUsersQueryResponse Handle()
    {
        return new()
        {
            Items = [
                new User { Username = "lionel57", Id = 27 },
                new User { Username = "catmaster", Id = 3 }
            ]
        };
    }
}