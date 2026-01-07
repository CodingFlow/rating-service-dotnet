using System.Text.Json.Nodes;
using RatingService.Application.Handlers;
using RatingService.Application.Queries;
using Service.Application.Common.Handlers;

namespace RatingService.Application;

internal class DeleteRatingsHandlerProxy(IDeleteRatingsHandler handler) : IHandler<DeleteRatingsQuery, JsonObject>
{
    async Task<JsonObject> IHandler<DeleteRatingsQuery, JsonObject>.Handle(DeleteRatingsQuery request)
    {
        await handler.Handle(request);
        return [];
    }
}
