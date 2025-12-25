using RatingService.Application.Queries;
using RatingService.Domain;

namespace RatingService.Application.Handlers;

internal class DeleteRatingsHandler(IRatingRepository ratingRepository) : IDeleteRatingsHandler
{
    public async Task Handle(DeleteRatingsQuery query)
    {
        if (query.Ids.Any())
        {
            await ratingRepository.Delete(query.Ids);
        } else
        {
            await ratingRepository.DeleteAll();
        }
    }
}