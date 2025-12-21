using RatingService.Application.Commands;
using RatingService.Domain;

namespace RatingService.Application.Handlers;

internal class PostRatingsHandler(IRatingRepository ratingRepository) : IPostRatingsHandler
{
    public async Task<string> Handle(PostRatingsCommand request)
    {
        var ratings = request.Items.Select(requestRating => new Rating
        {
            UserId = requestRating.UserId,
            ServiceId = requestRating.ServiceId,
            Score = requestRating.Score,
        });

        await ratingRepository.Add(ratings);

        await ratingRepository.Save();

        return await Task.FromResult(string.Empty);
    }
}