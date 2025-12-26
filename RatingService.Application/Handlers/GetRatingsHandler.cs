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

        var responseRatings = ratings.Select(rating => new Models.Rating
        {
            Id = rating.Id,
            UserId = rating.UserId,
            ServiceId = rating.ServiceId,
            Score = rating.Score,
        }).ToBlockingEnumerable();

        return new()
        {
            Items = responseRatings.ToList(),
        };
    }
}