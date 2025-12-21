using RatingService.Application.QueryResponses;
using RatingService.Domain;

namespace RatingService.Application.Handlers;

internal class GetRatingsHandler(IRatingRepository ratingRepository) : IGetRatingsHandler
{
    public async Task<GetRatingsQueryResponse> Handle()
    {
        var ratings = ratingRepository.FindAll();
        var responseRatings = ratings.Select(rating => new Models.Rating
        {
            Id = rating.Id,
            UserId = rating.UserId,
            ServiceId = rating.ServiceId,
            Score = rating.Score,
        }).ToBlockingEnumerable();

        return new()
        {
            Items = responseRatings
        };
    }
}