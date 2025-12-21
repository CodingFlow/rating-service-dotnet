using RatingService.Application.Commands;
using RatingService.Domain;

namespace RatingService.Application.Handlers;

internal class PostRatingsHandler(IRatingRepository ratingRepository) : IPostRatingsHandler
{
    public async Task<string> Handle(PostRatingsCommand request)
    {
        var requestRating = request.Items.First();
        var rating = new Rating
        {
            UserId = requestRating.UserId,
            ServiceId = requestRating.ServiceId,
            Score = requestRating.Score,
        };

        await ratingRepository.Add([rating]);

        await ratingRepository.Save();

        return await Task.FromResult(string.Empty);
    }
}