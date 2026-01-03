using RatingService.Application.Queries;
using RatingService.Application.QueryResponses;
using RatingService.Domain;

namespace RatingService.Application.Handlers;

internal class GetRatingsHandler(IRatingReadOnlyRepository ratingRepository) : IGetRatingsHandler
{
    public async Task<GetRatingsQueryResponse> Handle(GetRatingsQuery query)
    {
        var ratings = query.Ids.Any()
            ? ratingRepository.Find(query.Ids)
            : ratingRepository.FindAll();

        var responseRatings = await ratings.Select(rating => new Models.Rating
        {
            Id = rating.Id.Value,
            UserId = rating.UserId,
            ServiceId = rating.ServiceId,
            Score = rating.Score,
        }).ToListAsync();

        return new()
        {
            Items = responseRatings
        };
    }
}