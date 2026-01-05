using RatingService.Application.Commands;
using RatingService.Domain;

namespace RatingService.Application.Handlers;

internal class PostRatingsHandler(IRatingRepository ratingRepository) : IPostRatingsHandler
{
    public async Task Handle(PostRatingsCommand request)
    {
        var ratings = request.Items.Select(requestRating => new Rating
        {
            UserId = UserId.From(requestRating.UserId),
            ServiceId = ServiceId.From(requestRating.ServiceId),
            Score = Score.From(requestRating.Score),
        });

        await ratingRepository.Add(ratings);

        await ratingRepository.Save();
    }
}