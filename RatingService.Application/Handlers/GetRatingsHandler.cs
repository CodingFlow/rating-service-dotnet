using RatingService.Application.QueryResponses;

namespace RatingService.Application.Handlers;

internal class GetRatingsHandler : IGetRatingsHandler
{
    public GetRatingsQueryResponse Handle()
    {
        return new()
        {
            Items = [
                new() {
                    Id = 5,
                    UserId = 1,
                    ServiceId = 3,
                    Score = 2
                }
            ]
        };
    }
}