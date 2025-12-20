using RatingService.Application.QueryResponses;

namespace RatingService.Application.Handlers;

internal class GetRatingsHandler : IGetRatingsHandler
{
    public async Task<GetRatingsQueryResponse> Handle()
    {
        return await Task.FromResult(new GetRatingsQueryResponse()
        {
            Items = [
                new() {
                    Id = 5,
                    UserId = 1,
                    ServiceId = 3,
                    Score = 2
                }
            ]
        });
    }
}