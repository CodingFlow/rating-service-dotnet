using RatingService.Application.Commands;

namespace RatingService.Application.Handlers;

internal class PostRatingsHandler : IPostRatingsHandler
{
    public string Handle(PostRatingsCommand request)
    {
        return string.Empty;
    }
}