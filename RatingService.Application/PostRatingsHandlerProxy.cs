using System.Text.Json.Nodes;
using RatingService.Application.Commands;
using RatingService.Application.Handlers;
using Service.Application.Common.Handlers;

namespace RatingService.Application;

internal class PostRatingsHandlerProxy(IPostRatingsHandler handler) : IHandler<PostRatingsCommand, JsonObject>
{
    async Task<JsonObject> IHandler<PostRatingsCommand, JsonObject>.Handle(PostRatingsCommand request)
    {
        await handler.Handle(request);
        return [];
    }
}
